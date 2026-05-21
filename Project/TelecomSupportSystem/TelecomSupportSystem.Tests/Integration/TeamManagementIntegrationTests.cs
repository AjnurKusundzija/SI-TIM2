using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Integration
{
    /// <summary>
    /// US-23 / US-24 – Team management integration tests:
    /// Controller → Service → Repository → InMemory DB.
    /// </summary>
    public class TeamManagementIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static UserController CreateController(
            ApplicationDbContext context, int userId, string role)
        {
            var service = new UserService(
                new TicketRepository(context),
                new UserRepository(context),
                new Mock<IPackageService>().Object,
                new TeamRepository(context));

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

        private static User MakeUser(int id, Role role, int? teamId = null) => new()
        {
            UserId        = id,
            FirstName     = "Test",
            LastName      = $"User{id}",
            Email         = $"u{id}@test.ba",
            Username      = $"u{id}",
            PasswordHash  = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role          = role,
            TeamId        = teamId,
        };

        // ─── Full reassignment: admin reads agent profile from DB ─────────────

        /// <summary>
        /// US-23: Admin can retrieve agent profile (required to read current team assignment).
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldReturnProfile_WhenAdminAccessesAgentProfile()
        {
            using var context = CreateDbContext();
            var admin = MakeUser(1, Role.ADMINISTRATOR);
            var agent = MakeUser(2, Role.AGENT, teamId: 10);
            context.Users.AddRange(admin, agent);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.GetUserProfile(2);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var profile = ok.Value.Should().BeOfType<UserProfileDto>().Subject;
            profile.UserId.Should().Be(2);
            profile.Role.Should().Be("AGENT");
        }

        /// <summary>
        /// US-23: Agent can retrieve profile of an agent in a different team.
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldReturnProfile_WhenAgentAccessesOtherAgentProfile()
        {
            using var context = CreateDbContext();
            var agent1 = MakeUser(1, Role.AGENT, teamId: 10);
            var agent2 = MakeUser(2, Role.AGENT, teamId: 20);
            context.Users.AddRange(agent1, agent2);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "AGENT");
            var result = await controller.GetUserProfile(2);

            result.Should().BeOfType<OkObjectResult>();
        }

        // ─── Non-admin attempt → 403 ──────────────────────────────────────────

        /// <summary>
        /// US-24: Client cannot access another user's profile → 403 Forbid.
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldReturnForbid_WhenClientAccessesOtherUserProfile()
        {
            using var context = CreateDbContext();
            var client1 = MakeUser(1, Role.CLIENT);
            var client2 = MakeUser(2, Role.CLIENT);
            context.Users.AddRange(client1, client2);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetUserProfile(2);

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// US-24: Non-existent user → 404 Not Found (not a 403).
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldReturn404_WhenUserDoesNotExist()
        {
            using var context = CreateDbContext();
            var admin = MakeUser(1, Role.ADMINISTRATOR);
            context.Users.Add(admin);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.GetUserProfile(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
