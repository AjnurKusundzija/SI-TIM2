using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Auth;
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
    // PB-51 — Sigurnosni testovi za upravljanje korisničkim nalozima (US-73, US-74, US-75, US-89)
    public class UserAccountManagementSecurityTests
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
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Role, role),
                    }, "Test"))
                }
            };
            return controller;
        }

        private static AuthController CreateAuthController(ApplicationDbContext context)
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = "sprint9-security-secret-key-must-be-32-chars!!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                })
                .Build();
            var service = new AuthService(new UserRepository(context), new RefreshTokenRepository(context), cfg);
            return new AuthController(service);
        }

        private static User MakeUser(int id, Role role, AccountStatus status = AccountStatus.ACTIVE) => new()
        {
            UserId = id,
            FirstName = $"Name{id}",
            LastName = $"Last{id}",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword),
            Phone = "061000000",
            Location = Location.SARAJEVO,
            Role = role,
            AccountStatus = status,
        };

        // ── US-73: Klijent i Agent ne smiju kreirati korisnika kroz formu ──────

        [Theory]
        [InlineData("CLIENT")]
        [InlineData("AGENT")]
        [InlineData("TECHNICIAN")]
        public async Task CreateUser_ShouldReturnForbid_WhenNonAdminAttempts(string role)
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Enum.Parse<Role>(role)));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, role);
            var result = await controller.CreateUser(new CreateUserDto
            {
                FirstName = "Z",
                LastName = "X",
                Email = "novi.za@test.ba",
                Password = TestPassword,
                Role = Role.CLIENT,
            });

            result.Should().BeOfType<ForbidResult>();
            (await context.Users.FirstOrDefaultAsync(u => u.Email == "novi.za@test.ba")).Should().BeNull();
        }

        // ── US-75/US-89: Deaktivirani korisnik ne može se prijaviti ────────────

        [Fact]
        public async Task DeactivatedUser_CannotLogin()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(50, Role.CLIENT, AccountStatus.INACTIVE));
            await context.SaveChangesAsync();

            var auth = CreateAuthController(context);
            var result = await auth.Login(new LoginRequestDto { Email = "u50@test.ba", Password = TestPassword });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // ── US-89: Agent ne može doći do liste agenata sa filterom AGENT
        //          jer GetAgentTeams je admin-only.
        [Fact]
        public async Task GetAgentTeams_ShouldReturnForbid_WhenAgentRoleAttempts()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.AGENT));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "AGENT");
            var result = await controller.GetAgentTeams();

            result.Should().BeOfType<ForbidResult>();
        }

        // ── US-74: Lista korisnika i detalji su zaštićeni — klijent ne smije ───

        [Fact]
        public async Task GetUsersList_ShouldReturnForbid_WhenClientAttempts()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.CLIENT));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "CLIENT");
            var result = await controller.GetUsersList(null, null, null, null, null, 1, 10);

            result.Should().BeOfType<ForbidResult>();
        }

        // ── US-74: Response DTO ne smije nikad sadržavati lozinku ──────────────

        [Fact]
        public async Task UserListItemDto_ShouldNeverContainPasswordOrHash()
        {
            var dto = new UserListItemDto();
            var passwordProp = dto.GetType().GetProperty("PasswordHash")
                ?? dto.GetType().GetProperty("Password");

            passwordProp.Should().BeNull("response DTO ne smije izložiti lozinku");
        }
    }
}
