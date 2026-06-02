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
    /// US-41 through US-48 – Reports/statistics integration tests:
    /// Controller → Service → Repository → InMemory DB.
    /// Tests GET /api/users/me/statistics endpoint.
    /// </summary>
    public class ReportIntegrationTests
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

        // ─── GET statistics endpoint returns grouped data from InMemory DB ──

        /// <summary>
        /// US-48: Agent requests statistics → 200, correct open/closed counts from DB.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn200WithCounts_WhenAgentRequestsStats()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            context.Users.AddRange(client, agent);

            // 2 OPEN tickets assigned to agent
            var t1 = new Ticket
            {
                TicketId = 1, Title = "T1", Description = "D", CreatorId = 1,
                Status = TicketStatus.OPEN, Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow.AddHours(-2),
            };
            var t2 = new Ticket
            {
                TicketId = 2, Title = "T2", Description = "D", CreatorId = 1,
                Status = TicketStatus.CLOSED, Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddHours(-4), ClosedDate = DateTime.UtcNow.AddHours(-1),
            };
            context.Tickets.AddRange(t1, t2);
            context.Set<TicketUser>().AddRange(
                new TicketUser { TicketId = 1, UserId = 2, AssignmentDate = DateTime.UtcNow.AddHours(-2), AssignmentType = AssignmentType.AUTOMATIC },
                new TicketUser { TicketId = 2, UserId = 2, AssignmentDate = DateTime.UtcNow.AddHours(-4), AssignmentType = AssignmentType.AUTOMATIC }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "AGENT");
            var result = await controller.GetMyStatistics();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var stats = ok.Value.Should().BeOfType<AgentStatisticsDto>().Subject;
            stats.OpenTicketsCount.Should().Be(1);
            stats.ClosedTicketsCount.Should().Be(1);
        }

        /// <summary>
        /// US-48: Technician can also access their own statistics.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn200_WhenTechnicianRequestsStats()
        {
            using var context = CreateDbContext();
            var tech = MakeUser(3, Role.TECHNICIAN);
            context.Users.Add(tech);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 3, role: "TECHNICIAN");
            var result = await controller.GetMyStatistics();

            result.Should().BeOfType<OkObjectResult>();
        }

        // ─── Client role → 403 ────────────────────────────────────────────────

        /// <summary>
        /// US-48: CLIENT cannot access statistics endpoint → 403 Forbid.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn403_WhenClientRequestsStats()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            context.Users.Add(client);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetMyStatistics();

            result.Should().BeOfType<ForbidResult>();
        }

        /// <summary>
        /// US-48: ADMINISTRATOR role cannot access agent statistics endpoint → 403 Forbid.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldReturn403_WhenAdminRequestsStats()
        {
            using var context = CreateDbContext();
            var admin = MakeUser(1, Role.ADMINISTRATOR);
            context.Users.Add(admin);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.GetMyStatistics();

            result.Should().BeOfType<ForbidResult>();
        }

        // ─── Statistics persist: average resolution time from DB ─────────────

        /// <summary>
        /// US-44: Average resolution time calculated from DB closed ticket timestamps.
        /// </summary>
        [Fact]
        public async Task GetMyStatistics_ShouldCalculateAvgResolutionTime_FromClosedTicketDates()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, Role.CLIENT);
            var agent  = MakeUser(2, Role.AGENT);
            context.Users.AddRange(client, agent);

            var created  = DateTime.UtcNow.AddHours(-6);
            var closed   = DateTime.UtcNow.AddHours(-2);  // 4h resolution

            var ticket = new Ticket
            {
                TicketId = 10, Title = "Closed ticket", Description = "D", CreatorId = 1,
                Status = TicketStatus.CLOSED, Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = created, ClosedDate = closed,
            };
            context.Tickets.Add(ticket);
            context.Set<TicketUser>().Add(new TicketUser
            {
                TicketId = 10, UserId = 2,
                AssignmentDate = created,
                AssignmentType = AssignmentType.AUTOMATIC,
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2, role: "AGENT");
            var result = await controller.GetMyStatistics();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var stats = ok.Value.Should().BeOfType<AgentStatisticsDto>().Subject;
            stats.AvgResolutionHours.Should().BeApproximately(4.0, 0.2);
        }
    }
}
