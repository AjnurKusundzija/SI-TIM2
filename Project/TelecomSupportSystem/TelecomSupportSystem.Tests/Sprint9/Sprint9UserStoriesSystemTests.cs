using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Auth;
using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-45, PB-50, PB-51 — sistemski testovi koji prolaze kroz sve slojeve.
    // Verifikuju konzistentnost između Controller → Service → Repository → InMemory DB.
    public class Sprint9UserStoriesSystemTests
    {
        private const string TestPassword = "StrongPass!23";

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static UserController CreateUserController(ApplicationDbContext context, int userId, string role)
        {
            var service = new UserService(
                new TicketRepository(context),
                new UserRepository(context),
                new Mock<IPackageService>().Object,
                new TeamRepository(context),
                new Mock<ITicketService>().Object,
                new Mock<INotificationService>().Object);
            var controller = new UserController(service);
            SetClaims(controller, userId, role);
            return controller;
        }

        private static AdminController CreateAdminController(ApplicationDbContext context, int userId, string role)
        {
            var controller = new AdminController(new ReportService(new ReportRepository(context)));
            SetClaims(controller, userId, role);
            return controller;
        }

        private static ReportsController CreateReportsController(ApplicationDbContext context, int userId, string role)
        {
            var controller = new ReportsController(new ReportService(new ReportRepository(context)));
            SetClaims(controller, userId, role);
            return controller;
        }

        private static AuthController CreateAuthController(ApplicationDbContext context)
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = "sprint9-system-secret-key-must-be-32-chars!!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                })
                .Build();
            var service = new AuthService(new UserRepository(context), new RefreshTokenRepository(context), cfg);
            return new AuthController(service);
        }

        private static void SetClaims(ControllerBase controller, int id, string role) =>
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                        new Claim(ClaimTypes.Role, role),
                    }, "Test"))
                }
            };

        private static User MakeUser(int id, Role role, AccountStatus status = AccountStatus.ACTIVE) => new()
        {
            UserId = id,
            FirstName = $"N{id}",
            LastName = $"L{id}",
            Email = $"u{id}@t",
            Username = $"u{id}",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword),
            Role = role,
            AccountStatus = status,
        };

        // PB-51 — Full lifecycle: admin kreira → klijent se prijavi → admin deaktivira → klijent ne može login
        [Fact]
        public async Task PB51_CreateLoginDeactivateBlockedLogin_WorksThroughControllerServiceRepo()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            await context.SaveChangesAsync();

            var userController = CreateUserController(context, 1, "ADMINISTRATOR");
            var auth = CreateAuthController(context);

            // 1. Kreiraj klijenta
            var createResult = await userController.CreateUser(new CreateUserDto
            {
                FirstName = "Sys",
                LastName = "Test",
                Email = "sys.test@t.ba",
                Phone = "061111000",
                Password = TestPassword,
                Role = Role.CLIENT,
                Location = Location.SARAJEVO,
            });
            createResult.Should().BeOfType<OkObjectResult>();
            var created = await context.Users.FirstAsync(u => u.Email == "sys.test@t.ba");

            // 2. Login uspijeva
            var loginOk = await auth.Login(new LoginRequestDto { Email = "sys.test@t.ba", Password = TestPassword });
            loginOk.Should().BeOfType<OkObjectResult>();

            // 3. Deaktivacija
            (await userController.DeactivateUser(created.UserId)).Should().BeOfType<OkObjectResult>();

            // 4. Login odbijen
            var loginAfter = await auth.Login(new LoginRequestDto { Email = "sys.test@t.ba", Password = TestPassword });
            loginAfter.Should().BeOfType<UnauthorizedObjectResult>();

            // 5. Reaktivacija → login ponovno radi
            (await userController.ReactivateUser(created.UserId)).Should().BeOfType<OkObjectResult>();
            (await auth.Login(new LoginRequestDto { Email = "sys.test@t.ba", Password = TestPassword }))
                .Should().BeOfType<OkObjectResult>();
        }

        // PB-45 / PB-50 — Dashboard agregat odražava stvarne tikete u bazi
        [Fact]
        public async Task PB45_DashboardAggregates_ReflectActualTicketsInDb()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.ADMINISTRATOR), MakeUser(2, Role.CLIENT), MakeUser(3, Role.AGENT));
            var now = DateTime.UtcNow;
            context.Tickets.AddRange(
                new Ticket { TicketId = 1, Title = "T1", Description = "D", CreatorId = 2, Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = now.AddDays(-1) },
                new Ticket { TicketId = 2, Title = "T2", Description = "D", CreatorId = 2, Status = TicketStatus.CLOSED, Priority = Priority.LOW, ProblemCategory = ProblemCategory.BILLING, CreatedDate = now.AddDays(-2), ClosedDate = now.AddDays(-1) });
            context.Comments.Add(new Comment { CommentId = 100, TicketId = 1, AuthorId = 3, Content = "reply", DateTime = now.AddDays(-1).AddMinutes(40) });
            await context.SaveChangesAsync();

            var adminCtrl = CreateAdminController(context, 1, "ADMINISTRATOR");
            var dashboard = ((OkObjectResult)await adminCtrl.GetDashboard("month", null, null)).Value
                .Should().BeOfType<AdminDashboardDto>().Subject;

            dashboard.TotalTicketsInPeriod.Should().Be(2);
            dashboard.AvgFirstResponseMinutes.Should().NotBeNull();
            dashboard.ActiveUsersByRole.Agents.Should().Be(1);
            dashboard.ActiveUsersByRole.Clients.Should().Be(1);
        }

        // PB-45 / US-72 — Custom range invalid → BadRequest i ne dohvata podatke
        [Fact]
        public async Task PB45_GlobalFilter_RejectsInvalidCustomRange_WithoutData()
        {
            using var context = CreateDbContext();
            var controller = CreateAdminController(context, 1, "ADMINISTRATOR");

            var result = await controller.GetDashboard("custom", DateTime.UtcNow, DateTime.UtcNow.AddDays(-10));

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // PB-50 / US-88 — FIRST_RESPONSE izvještaj ima bucket-e konsistentne sa periodom
        [Fact]
        public async Task PB50_GenerateFirstResponseReport_HasBucketsAndAvgConsistentWithDb()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.CLIENT), MakeUser(3, Role.AGENT));
            var now = DateTime.UtcNow;
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = now.AddDays(-1),
            });
            context.Comments.Add(new Comment { CommentId = 1, TicketId = 1, AuthorId = 3, Content = "r", DateTime = now.AddDays(-1).AddMinutes(15) });
            await context.SaveChangesAsync();

            var ctrl = CreateReportsController(context, 1, "ADMINISTRATOR");
            var result = await ctrl.GenerateReport(new ReportRequestDto { ReportType = ReportType.FIRST_RESPONSE, Period = "week" });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            report.HasData.Should().BeTrue();
            var data = report.Data.Should().BeOfType<FirstResponseReportDto>().Subject;
            data.TotalTicketsCount.Should().Be(1);
            data.TicketsWithResponseCount.Should().Be(1);
            data.AvgFirstResponseMinutes.Should().NotBeNull();
            data.Buckets.Should().NotBeEmpty();
        }
    }
}
