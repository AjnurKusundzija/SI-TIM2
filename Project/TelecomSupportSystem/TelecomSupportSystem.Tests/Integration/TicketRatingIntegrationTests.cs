using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Ratings;
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
    /// US-18 – Ticket rating integration tests:
    /// Controller → Service → Repository → InMemory DB.
    /// </summary>
    public class TicketRatingIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketRatingController CreateController(
            ApplicationDbContext context, int userId, string role)
        {
            var ratingRepo = new RatingRepository(context);
            var ticketRepo = new TicketRepository(context);
            var service    = new RatingService(ratingRepo, ticketRepo);

            var controller = new TicketRatingController(service);
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

        private static Ticket MakeTicket(int id, int creatorId, TicketStatus status) => new()
        {
            TicketId        = id,
            Title           = $"Ticket {id}",
            Description     = "Desc",
            CreatorId       = creatorId,
            Status          = status,
            Priority        = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow.AddDays(-5),
            ClosedDate      = status == TicketStatus.CLOSED ? DateTime.UtcNow.AddDays(-1) : null,
        };

        // ─── POST rating on closed ticket → 201 Created ─────────────────────

        /// <summary>
        /// US-18: Client rates own CLOSED ticket → 201, rating persisted in DB.
        /// </summary>
        [Fact]
        public async Task CreateRating_ShouldReturn201AndPersist_WhenClientRatesOwnClosedTicket()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeTicket(10, creatorId: 1, TicketStatus.CLOSED);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 5, RatingComment = "Great service!" };

            var result = await controller.CreateRating(10, dto);

            result.Should().BeOfType<CreatedAtActionResult>();
            var saved = await context.Ratings.FirstOrDefaultAsync(r => r.TicketId == 10);
            saved.Should().NotBeNull();
            saved!.RatingValue.Should().Be(5);
            saved.UserId.Should().Be(1);
        }

        // ─── POST rating on open ticket → 409 Conflict ───────────────────────

        /// <summary>
        /// US-18: Client cannot rate OPEN ticket → 409 Conflict (InvalidOperationException).
        /// </summary>
        [Fact]
        public async Task CreateRating_ShouldReturn409_WhenTicketIsNotClosed()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeTicket(20, creatorId: 1, TicketStatus.OPEN);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 3 };

            var result = await controller.CreateRating(20, dto);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        // ─── POST rating on another user's ticket → 403 Forbid ───────────────

        /// <summary>
        /// US-18: Client cannot rate another client's ticket → 403 Forbid.
        /// </summary>
        [Fact]
        public async Task CreateRating_ShouldReturn403_WhenClientRatesAnotherUsersTicket()
        {
            using var context = CreateDbContext();
            var owner  = MakeUser(1, Role.CLIENT);
            var other  = MakeUser(2, Role.CLIENT);
            var ticket = MakeTicket(30, creatorId: 1, TicketStatus.CLOSED);
            context.Users.AddRange(owner, other);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            // userId: 2 tries to rate ticket owned by userId: 1
            var controller = CreateController(context, userId: 2, role: "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 4 };

            var result = await controller.CreateRating(30, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── POST duplicate rating → 409 Conflict ────────────────────────────

        /// <summary>
        /// US-18: Second rating on same ticket → 409 Conflict.
        /// </summary>
        [Fact]
        public async Task CreateRating_ShouldReturn409_WhenTicketAlreadyRated()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeTicket(40, creatorId: 1, TicketStatus.CLOSED);
            var existing = new TelecomSupportSystem.DAL.Entities.Rating
            {
                TicketId      = 40,
                UserId        = 1,
                RatingValue   = 3,
                RatingComment = "",
                RatingDate    = DateTime.UtcNow,
            };
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            context.Ratings.Add(existing);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 5 };

            var result = await controller.CreateRating(40, dto);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        // ─── Rating value out of valid range → 400 Bad Request ───────────────

        /// <summary>
        /// US-18: Rating value 0 (below minimum) — ModelState invalid → controller
        /// short-circuits with 400 Bad Request.
        /// </summary>
        [Fact]
        public async Task CreateRating_ShouldReturn400_WhenRatingValueIsZero()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var ticket = MakeTicket(50, creatorId: 1, TicketStatus.CLOSED);
            context.Users.Add(client);
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            controller.ModelState.AddModelError("RatingValue", "Rating must be between 1 and 5.");
            var dto = new CreateRatingDto { RatingValue = 0 };

            var result = await controller.CreateRating(50, dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─── GET rating — agent can view ──────────────────────────────────────

        /// <summary>
        /// US-18: Agent can retrieve the rating for any closed ticket.
        /// </summary>
        [Fact]
        public async Task GetRating_ShouldReturnRating_WhenAgentRequests()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            var ticket = MakeTicket(60, creatorId: 1, TicketStatus.CLOSED);
            var rating = new TelecomSupportSystem.DAL.Entities.Rating
            {
                TicketId      = 60,
                UserId        = 1,
                RatingValue   = 4,
                RatingComment = "Good",
                RatingDate    = DateTime.UtcNow,
            };
            context.Users.AddRange(client, agent);
            context.Tickets.Add(ticket);
            context.Ratings.Add(rating);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "AGENT");
            var result = await controller.GetRating(60);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<RatingDto>().Subject;
            dto.RatingValue.Should().Be(4);
        }
    }
}
