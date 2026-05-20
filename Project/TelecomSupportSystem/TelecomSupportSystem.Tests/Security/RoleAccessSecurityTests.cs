using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Security
{
    /// <summary>
    /// RBAC regression tests — verifies that role-based access control is
    /// enforced correctly across ticket and user endpoints.
    /// </summary>
    public class RoleAccessSecurityTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateTicketController(
            ApplicationDbContext context, int userId, string role)
        {
            var service = new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object,
                new Mock<ICommentService>().Object);

            var controller = new TicketController(service);
            SetClaims(controller, userId, role);
            return controller;
        }

        private static UserController CreateUserController(
            ApplicationDbContext context, int userId, string role)
        {
            var service = new UserService(
                new TicketRepository(context),
                new UserRepository(context),
                new Mock<IPackageService>().Object);

            var controller = new UserController(service);
            SetClaims(controller, userId, role);
            return controller;
        }

        private static void SetClaims(ControllerBase controller, int userId, string role)
        {
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
        }

        private static User MakeUser(int id, Role role) => new()
        {
            UserId        = id,
            FirstName     = "Test",
            LastName      = $"User{id}",
            Email         = $"u{id}@test.ba",
            Username      = $"u{id}",
            PasswordHash  = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role          = role,
        };

        private static Ticket MakeOpenTicket(int id, int creatorId) => new()
        {
            TicketId        = id,
            Title           = $"Ticket {id}",
            Description     = "D",
            CreatorId       = creatorId,
            Status          = TicketStatus.OPEN,
            Priority        = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow,
        };

        // ─── Client cannot access agent-only ticket endpoint ──────────────────

        /// <summary>
        /// RBAC: Client cannot list all tickets (agent endpoint) → 403.
        /// </summary>
        [Fact]
        public async Task GetAllTickets_ShouldReturn403_WhenClientAccessesAgentEndpoint()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            context.Users.Add(client);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetAllTickets();

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Client cannot access internal priority (staff-only) ─────────────

        /// <summary>
        /// RBAC: Client cannot set internal priority → 403.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn403_WhenClientAttempts()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeOpenTicket(10, creatorId: 1);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");
            var dto = new TelecomSupportSystem.BLL.DTOs.Tickets.UpdateInternalPriorityDto
            {
                Priority = InternalPriority.HIGH
            };

            var result = await controller.UpdateInternalPriority(10, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Technician cannot set internal priority ──────────────────────────

        /// <summary>
        /// RBAC: Technician cannot change internal priority (AGENT/ADMIN only) → 403.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn403_WhenTechnicianAttempts()
        {
            using var context = CreateDbContext();
            var tech   = MakeUser(1, Role.TECHNICIAN);
            var ticket = MakeOpenTicket(10, creatorId: 2);
            context.Users.Add(tech);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "TECHNICIAN");
            var dto = new TelecomSupportSystem.BLL.DTOs.Tickets.UpdateInternalPriorityDto
            {
                Priority = InternalPriority.HIGH
            };

            var result = await controller.UpdateInternalPriority(10, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Client cannot access another user's profile ─────────────────────

        /// <summary>
        /// RBAC: Client cannot read another user's profile → 403.
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldReturn403_WhenClientAccessesOtherUser()
        {
            using var context = CreateDbContext();
            var client1 = MakeUser(1, Role.CLIENT);
            var client2 = MakeUser(2, Role.CLIENT);
            context.Users.AddRange(client1, client2);
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetUserProfile(2);

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Agent cannot access admin statistics endpoint ────────────────────

        /// <summary>
        /// RBAC: Agent can access statistics (my own) — returns 200.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn200_WhenAgentRequestsOwnStats()
        {
            using var context = CreateDbContext();
            var agent = MakeUser(1, Role.AGENT);
            context.Users.Add(agent);
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, userId: 1, role: "AGENT");
            var result = await controller.GetMyStatistics();

            result.Should().BeOfType<OkObjectResult>();
        }

        // ─── Client cannot access statistics ─────────────────────────────────

        /// <summary>
        /// RBAC: Client cannot access statistics endpoint → 403.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn403_WhenClientRequestsStats()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            context.Users.Add(client);
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetMyStatistics();

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Only assigned staff can update status ────────────────────────────

        /// <summary>
        /// RBAC: Non-technician role (AGENT) cannot call UpdateStatus → 403.
        /// </summary>
        [Fact]
        public async Task UpdateStatus_ShouldReturn403_WhenRoleIsNotTechnician()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            var ticket = MakeOpenTicket(10, creatorId: 1);
            context.Users.AddRange(client, agent);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 2, role: "AGENT");
            var dto = new TelecomSupportSystem.BLL.DTOs.Tickets.UpdateTicketStatusDto
            {
                Status = TicketStatus.CLOSURE_REQUESTED
            };

            var result = await controller.UpdateStatus(10, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Technician response DTO security ─────────────────────────────────

        /// <summary>
        /// RBAC: Technician's view of ticket detail does not expose internal priority to clients.
        /// Verify UserProfileDto returned to technician does not include PasswordHash.
        /// </summary>
        [Fact]
        public async Task GetUserProfile_ShouldNotContainPasswordHash_InResponseDto()
        {
            using var context = CreateDbContext();
            var admin  = MakeUser(1, Role.ADMINISTRATOR);
            var client = MakeUser(2, Role.CLIENT);
            context.Users.AddRange(admin, client);
            await context.SaveChangesAsync();

            var controller = CreateUserController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.GetUserProfile(2);

            var ok  = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value!;

            var passwordProp = dto.GetType().GetProperty("PasswordHash")
                            ?? dto.GetType().GetProperty("Password");

            passwordProp.Should().BeNull(
                "response DTO must not expose password hash to any caller");
        }

        // ─── Closure workflow RBAC ────────────────────────────────────────────

        /// <summary>
        /// RBAC: Client cannot request ticket closure → 403.
        /// </summary>
        [Fact]
        public async Task RequestClosure_ShouldReturn403_WhenClientAttempts()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeOpenTicket(20, creatorId: 1);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");
            var result = await controller.RequestClosure(20);

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// RBAC: Only AGENT/TECHNICIAN/ADMIN can access assigned-only tickets; CLIENT gets 403.
        /// </summary>
        [Fact]
        public async Task GetOpenAssignedTickets_ShouldReturn403_WhenClientAttempts()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            context.Users.Add(client);
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetOpenAssignedTickets();

            result.Should().BeOfType<ForbidResult>();
        }
    }
}
