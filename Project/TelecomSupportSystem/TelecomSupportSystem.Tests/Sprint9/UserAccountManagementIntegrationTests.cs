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
    // PB-51 — Integracijski testovi Controller -> Service -> Repository -> InMemory DB
    // Pokrivaju US-73 (kreiranje), US-74 (uređivanje + lista), US-75 (deaktivacija klijenata),
    // US-89 (upravljanje agentima).
    public class UserAccountManagementIntegrationTests
    {
        private const string TestPassword = "StrongPass!23";

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static IConfiguration BuildAuthConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = "sprint9-integration-secret-key-must-be-32-chars!!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                })
                .Build();

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
            var service = new AuthService(
                new UserRepository(context),
                new RefreshTokenRepository(context),
                BuildAuthConfig());
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

        // ── US-73: end-to-end kreiranje ─────────────────────────────────────────

        [Fact]
        public async Task CreateUser_EndToEnd_ShouldPersistAndAllowLogin_WhenAdminCreatesClient()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            var dto = new CreateUserDto
            {
                FirstName = "Novi",
                LastName = "Klijent",
                Email = "novi.klijent@test.ba",
                Phone = "061123456",
                Password = TestPassword,
                Role = Role.CLIENT,
                Location = Location.TUZLA,
            };

            var result = await controller.CreateUser(dto);
            result.Should().BeOfType<OkObjectResult>();

            var created = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            created.Should().NotBeNull();
            created!.AccountStatus.Should().Be(AccountStatus.ACTIVE);
            created.PasswordHash.Should().NotBe(TestPassword);

            // Smoke: login uspijeva za kreiranog klijenta
            var auth = CreateAuthController(context);
            var login = await auth.Login(new LoginRequestDto { Email = dto.Email, Password = TestPassword });
            login.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateUser_EndToEnd_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.ADMINISTRATOR), MakeUser(2, Role.CLIENT));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            var result = await controller.CreateUser(new CreateUserDto
            {
                FirstName = "Dup",
                LastName = "User",
                Email = "u2@test.ba",
                Password = TestPassword,
                Role = Role.CLIENT,
                Phone = "061",
            });

            result.Should().BeOfType<ConflictObjectResult>();
        }

        // ── US-74: lista, paginacija, filteri ──────────────────────────────────

        [Fact]
        public async Task GetUsersList_EndToEnd_ShouldReturnPagedActiveUsers_WhenAdminCallsWithoutFilters()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                MakeUser(1, Role.ADMINISTRATOR),
                MakeUser(2, Role.CLIENT),
                MakeUser(3, Role.AGENT),
                MakeUser(4, Role.TECHNICIAN),
                MakeUser(5, Role.CLIENT, AccountStatus.INACTIVE));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");

            var result = await controller.GetUsersList(null, "ACTIVE", null, null, null, 1, 10);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var list = ok.Value.Should().BeOfType<UserListDto>().Subject;
            list.Users.Should().OnlyContain(u => u.AccountStatus == "ACTIVE");
            list.Users.Should().HaveCount(4);
            list.TotalCount.Should().Be(4);
        }

        [Fact]
        public async Task GetUsersList_EndToEnd_ShouldFilterByLocationAndSearch()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                MakeUser(1, Role.ADMINISTRATOR),
                new User { UserId = 10, FirstName = "Marko", LastName = "Markovic", Email = "marko@test.ba", Username = "m1", PasswordHash = "h", Phone = "061222333", Location = Location.SARAJEVO, Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 11, FirstName = "Ivana", LastName = "Ivanic", Email = "ivana@test.ba", Username = "m2", PasswordHash = "h", Phone = "061333444", Location = Location.MOSTAR, Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE });
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");

            var byLocation = await controller.GetUsersList(null, "ACTIVE", null, null, "MOSTAR", 1, 10);
            var byLocationDto = ((OkObjectResult)byLocation).Value.Should().BeOfType<UserListDto>().Subject;
            byLocationDto.Users.Should().OnlyContain(u => u.Location == "MOSTAR");

            var byPhoneSearch = await controller.GetUsersList(null, "ACTIVE", null, "222", null, 1, 10);
            var byPhoneSearchDto = ((OkObjectResult)byPhoneSearch).Value.Should().BeOfType<UserListDto>().Subject;
            byPhoneSearchDto.Users.Should().ContainSingle(u => u.Phone == "061222333");
        }

        [Fact]
        public async Task UpdateUserDetails_EndToEnd_ShouldUpdate_WithoutChangingRole()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.ADMINISTRATOR), MakeUser(2, Role.CLIENT));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");

            var result = await controller.UpdateUserDetails(2, new UpdateUserDetailsDto
            {
                FirstName = "Novo",
                LastName = "Ime",
                Phone = "061999999",
                Location = Location.BIHAC,
            });

            result.Should().BeOfType<OkObjectResult>();
            var updated = await context.Users.FindAsync(2);
            updated!.FirstName.Should().Be("Novo");
            updated.Phone.Should().Be("061999999");
            updated.Location.Should().Be(Location.BIHAC);
            updated.Role.Should().Be(Role.CLIENT);
        }

        // ── US-75: deaktivacija klijenta + blokada login-a ─────────────────────

        [Fact]
        public async Task DeactivateClient_EndToEnd_ShouldSetInactiveAndBlockLogin()
        {
            using var context = CreateDbContext();
            var admin = MakeUser(1, Role.ADMINISTRATOR);
            var client = MakeUser(2, Role.CLIENT);
            context.Users.AddRange(admin, client);
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            var result = await controller.DeactivateUser(2);
            result.Should().BeOfType<OkObjectResult>();

            (await context.Users.FindAsync(2))!.AccountStatus.Should().Be(AccountStatus.INACTIVE);

            // Smoke: login deaktiviranog korisnika je odbijen
            var auth = CreateAuthController(context);
            var login = await auth.Login(new LoginRequestDto { Email = client.Email, Password = TestPassword });
            login.Should().BeOfType<UnauthorizedObjectResult>();
        }

        [Fact]
        public async Task DeactivateClient_EndToEnd_ShouldPreserveHistoricalTickets()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.ADMINISTRATOR), MakeUser(2, Role.CLIENT));
            context.Tickets.Add(new Ticket
            {
                TicketId = 100, Title = "Hist", Description = "D", CreatorId = 2,
                Status = TicketStatus.CLOSED, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddDays(-3), ClosedDate = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            await controller.DeactivateUser(2);

            var historical = await context.Tickets.FirstOrDefaultAsync(t => t.CreatorId == 2);
            historical.Should().NotBeNull();
            historical!.TicketId.Should().Be(100);
        }

        [Fact]
        public async Task DeactivateUser_EndToEnd_ShouldReturnForbid_WhenAgentDeactivatesAgent()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.AGENT), MakeUser(2, Role.AGENT));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "AGENT");
            var result = await controller.DeactivateUser(2);

            result.Should().BeOfType<ForbidResult>();
            (await context.Users.FindAsync(2))!.AccountStatus.Should().Be(AccountStatus.ACTIVE);
        }

        [Fact]
        public async Task DeactivateUser_EndToEnd_ShouldReturnBadRequest_WhenAdminDeactivatesOwnAccount()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            var result = await controller.DeactivateUser(1);

            result.Should().BeOfType<BadRequestObjectResult>();
            (await context.Users.FindAsync(1))!.AccountStatus.Should().Be(AccountStatus.ACTIVE);
        }

        // ── US-89: deaktivacija agenta sa otvorenim tiketima → BadRequest ──────

        [Fact]
        public async Task DeactivateAgent_EndToEnd_ShouldRejectWhenAgentHasOpenAssignedTickets()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.ADMINISTRATOR), MakeUser(2, Role.AGENT), MakeUser(3, Role.CLIENT));
            context.Tickets.Add(new Ticket
            {
                TicketId = 200, Title = "Open", Description = "D", CreatorId = 3,
                Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
            });
            context.TicketUsers.Add(new TicketUser
            {
                TicketId = 200, UserId = 2, AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.AUTOMATIC,
            });
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, 1, "ADMINISTRATOR");
            var result = await controller.DeactivateUser(2);

            result.Should().BeOfType<BadRequestObjectResult>();
            (await context.Users.FindAsync(2))!.AccountStatus.Should().Be(AccountStatus.ACTIVE);
        }

        [Fact]
        public async Task DeactivatedAgent_ShouldNotAppear_InForwardingCandidates()
        {
            using var context = CreateDbContext();
            // Setup: 2 agenta — jedan aktivan, jedan inactive — i jedan vlasnik tiketa
            context.Users.AddRange(
                new User { UserId = 1, FirstName = "A", LastName = "1", Email = "a1@t", Username = "a1", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, Location = Location.SARAJEVO },
                new User { UserId = 2, FirstName = "A", LastName = "2", Email = "a2@t", Username = "a2", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.INACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, Location = Location.SARAJEVO },
                new User { UserId = 3, FirstName = "Owner", LastName = "0", Email = "o@t", Username = "o", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE, AvailabilityStatus = AvailabilityStatus.AVAILABLE, Location = Location.SARAJEVO });
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);
            var candidates = await repo.GetAvailableAgentsForForwardingAsync(excludeUserId: 3);

            candidates.Should().OnlyContain(u => u.AccountStatus == AccountStatus.ACTIVE);
            candidates.Select(u => u.UserId).Should().Contain(1);
            candidates.Select(u => u.UserId).Should().NotContain(2);
        }
    }
}
