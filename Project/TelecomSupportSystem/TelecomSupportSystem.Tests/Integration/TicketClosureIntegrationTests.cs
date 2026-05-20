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

namespace TelecomSupportSystem.Tests.Integration
{
    /// <summary>
    /// US-16 / US-17 – End-to-end closure workflow:
    /// Controller → Service → Repository → InMemory DB.
    /// </summary>
    public class TicketClosureIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(
            ApplicationDbContext context, int userId, string role)
        {
            var ticketRepo = new TicketRepository(context);
            var service = new TicketService(
                ticketRepo,
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object,
                new Mock<ICommentService>().Object);

            var controller = new TicketController(service);
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

        private static Ticket MakeOpenTicket(int id, int creatorId, int agentId, ApplicationDbContext context)
        {
            var ticket = new Ticket
            {
                TicketId        = id,
                Title           = $"Ticket {id}",
                Description     = "Test",
                CreatorId       = creatorId,
                Status          = TicketStatus.OPEN,
                Priority        = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate     = DateTime.UtcNow.AddDays(-3),
            };
            context.Tickets.Add(ticket);
            context.Set<TicketUser>().Add(new TicketUser
            {
                TicketId       = id,
                UserId         = agentId,
                AssignmentDate = DateTime.UtcNow.AddDays(-3),
                AssignmentType = AssignmentType.AUTOMATIC,
            });
            return ticket;
        }

        // ─── Full accept-closure cycle ────────────────────────────────────────

        /// <summary>
        /// US-16 full cycle: agent requests closure → client accepts → ticket is CLOSED.
        /// </summary>
        [Fact]
        public async Task FullAcceptCycle_ShouldCloseTicket_WhenAgentRequestsAndClientAccepts()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            context.Users.AddRange(client, agent);
            MakeOpenTicket(10, creatorId: 1, agentId: 2, context);
            await context.SaveChangesAsync();

            // Step 1: agent requests closure
            var agentController = CreateController(context, userId: 2, role: "AGENT");
            var requestResult = await agentController.RequestClosure(10);
            requestResult.Should().BeOfType<OkObjectResult>();

            var afterRequest = await context.Tickets.FindAsync(10);
            afterRequest!.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            afterRequest.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);

            // Step 2: client accepts closure
            var clientController = CreateController(context, userId: 1, role: "CLIENT");
            var acceptResult = await clientController.AcceptClosure(10);
            acceptResult.Should().BeOfType<OkObjectResult>();

            var afterAccept = await context.Tickets.FindAsync(10);
            afterAccept!.Status.Should().Be(TicketStatus.CLOSED);
            afterAccept.ClosureRequestStatus.Should().Be(ClosureRequestStatus.ACCEPTED);
            afterAccept.ClosedDate.Should().NotBeNull();
        }

        // ─── Full reject-closure cycle ────────────────────────────────────────

        /// <summary>
        /// US-17 full cycle: agent requests closure → client rejects → ticket returns to OPEN.
        /// </summary>
        [Fact]
        public async Task FullRejectCycle_ShouldReopenTicket_WhenAgentRequestsAndClientRejects()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            context.Users.AddRange(client, agent);
            MakeOpenTicket(20, creatorId: 1, agentId: 2, context);
            await context.SaveChangesAsync();

            // Step 1: agent requests closure
            var agentController = CreateController(context, userId: 2, role: "AGENT");
            await agentController.RequestClosure(20);

            // Step 2: client rejects closure
            var clientController = CreateController(context, userId: 1, role: "CLIENT");
            var rejectResult = await clientController.RejectClosure(20);
            rejectResult.Should().BeOfType<OkObjectResult>();

            var afterReject = await context.Tickets.FindAsync(20);
            afterReject!.Status.Should().Be(TicketStatus.OPEN);
            afterReject.ClosureRequestStatus.Should().Be(ClosureRequestStatus.REJECTED);
        }

        // ─── Unauthorized closure attempt ────────────────────────────────────

        /// <summary>
        /// US-16: Client cannot request closure — returns 403 Forbid.
        /// </summary>
        [Fact]
        public async Task RequestClosure_ShouldReturnForbid_WhenRoleIsClient()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            context.Users.Add(client);
            MakeOpenTicket(30, creatorId: 1, agentId: 1, context);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.RequestClosure(30);

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// US-16: Unassigned agent cannot request closure — returns 403 Forbid.
        /// </summary>
        [Fact]
        public async Task RequestClosure_ShouldReturnForbid_WhenAgentIsNotAssigned()
        {
            using var context = CreateDbContext();
            var client       = MakeUser(1, Role.CLIENT);
            var assignedAgent = MakeUser(2, Role.AGENT);
            var otherAgent    = MakeUser(3, Role.AGENT);
            context.Users.AddRange(client, assignedAgent, otherAgent);
            MakeOpenTicket(40, creatorId: 1, agentId: 2, context);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 3, role: "AGENT");
            var result = await controller.RequestClosure(40);

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// US-16: Cannot request closure twice (already CLOSURE_REQUESTED) → returns 400.
        /// </summary>
        [Fact]
        public async Task RequestClosure_ShouldReturnBadRequest_WhenAlreadyInClosureRequestedState()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            context.Users.AddRange(client, agent);
            MakeOpenTicket(50, creatorId: 1, agentId: 2, context);
            await context.SaveChangesAsync();

            // First request succeeds
            var controller = CreateController(context, userId: 2, role: "AGENT");
            await controller.RequestClosure(50);

            // Second request should fail
            var result = await controller.RequestClosure(50);
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
