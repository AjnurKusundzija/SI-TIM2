using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
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
    /// US-21 / US-22 – Internal priority management integration tests:
    /// Controller → Service → Repository → InMemory DB.
    /// </summary>
    public class TicketPriorityIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(
            ApplicationDbContext context, int userId, string role)
        {
            var service = new TicketService(
                new TicketRepository(context),
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

        private static Ticket MakeTicket(int id, int creatorId) => new()
        {
            TicketId        = id,
            Title           = $"Ticket {id}",
            Description     = "Desc",
            CreatorId       = creatorId,
            Status          = TicketStatus.OPEN,
            Priority        = Priority.LOW,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow,
        };

        // ─── PATCH priority by agent → 200, persisted ─────────────────────────

        /// <summary>
        /// US-21: Agent sets internal priority → 200 OK, change persisted in DB.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn200AndPersist_WhenAgentSetsPriority()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            var ticket = MakeTicket(10, creatorId: 1);
            context.Users.AddRange(client, agent);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "AGENT");
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.HIGH };

            var result = await controller.UpdateInternalPriority(10, dto);

            result.Should().BeOfType<OkObjectResult>();
            var saved = await context.Tickets.FindAsync(10);
            saved!.InternalPriority.Should().Be(InternalPriority.HIGH);
        }

        /// <summary>
        /// US-21: Admin sets internal priority → 200 OK, change persisted in DB.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn200_WhenAdminSetsPriority()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var admin  = MakeUser(2, Role.ADMINISTRATOR);
            var ticket = MakeTicket(20, creatorId: 1);
            context.Users.AddRange(client, admin);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "ADMINISTRATOR");
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.CRITICAL };

            var result = await controller.UpdateInternalPriority(20, dto);

            result.Should().BeOfType<OkObjectResult>();
            var saved = await context.Tickets.FindAsync(20);
            saved!.InternalPriority.Should().Be(InternalPriority.CRITICAL);
        }

        // ─── PATCH priority by client → 403 ───────────────────────────────────

        /// <summary>
        /// US-21: Client cannot change internal priority → 403 Forbid.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn403_WhenClientAttempts()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeTicket(30, creatorId: 1);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.HIGH };

            var result = await controller.UpdateInternalPriority(30, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// US-21: Non-existent ticket → 404 Not Found.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriority_ShouldReturn404_WhenTicketDoesNotExist()
        {
            using var context = CreateDbContext();
            var agent = MakeUser(2, Role.AGENT);
            context.Users.Add(agent);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "AGENT");
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.MEDIUM };

            var result = await controller.UpdateInternalPriority(999, dto);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
