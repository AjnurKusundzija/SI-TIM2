using System.Diagnostics;
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

namespace TelecomSupportSystem.Tests.Performance.Scale
{
    public class AutoAssignScalePerformanceTests
    {
        private const int MaxAutoAssignTimeMilliseconds = 3000;

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context)
        {
            var controller = new TicketController(new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object, new Mock<ICommentService>().Object));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "CLIENT")
                    }, "Test"))
                }
            };

            return controller;
        }

        [Fact]
        public async Task CreateTicketWithAutoAssign_ShouldCompleteWithinThreeSeconds_WithLargeAgentAndHistorySet()
        {
            using var context = CreateDbContext();

            context.Teams.Add(new Team
            {
                TeamId = 10,
                TeamName = "Internet Scale Team",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });

            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Client",
                LastName = "Scale",
                Email = "client-scale@test.ba",
                Username = "client-scale",
                PasswordHash = "hash",
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE
            });

            var tickets = new List<Ticket>();
            var assignments = new List<TicketUser>();
            var nextTicketId = 10000;

            for (var i = 0; i < 60; i++)
            {
                var agentId = 100 + i;
                context.Users.Add(new User
                {
                    UserId = agentId,
                    FirstName = $"Agent{agentId}",
                    LastName = "Scale",
                    Email = $"agent{agentId}@test.ba",
                    Username = $"agent{agentId}",
                    PasswordHash = "hash",
                    Role = Role.AGENT,
                    AccountStatus = AccountStatus.ACTIVE,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    TeamId = 10
                });

                for (var j = 0; j < i % 26; j++)
                {
                    var ticketId = nextTicketId++;
                    tickets.Add(new Ticket
                    {
                        TicketId = ticketId,
                        Title = $"Existing {agentId}-{j}",
                        Description = "History",
                        CreatorId = 1,
                        CreatedDate = DateTime.UtcNow.AddMinutes(-ticketId),
                        Status = j % 5 == 0 ? TicketStatus.CLOSED : TicketStatus.OPEN,
                        Priority = (Priority)((j % 3) + 1),
                        ProblemCategory = ProblemCategory.INTERNET
                    });
                    assignments.Add(new TicketUser
                    {
                        TicketId = ticketId,
                        UserId = agentId,
                        TeamId = 10,
                        AssignmentDate = DateTime.UtcNow.AddMinutes(-j),
                        AssignmentType = AssignmentType.MANUAL,
                        Note = "Scale history"
                    });
                }
            }

            context.Tickets.AddRange(tickets);
            context.Set<TicketUser>().AddRange(assignments);
            await context.SaveChangesAsync();

            var stopwatch = Stopwatch.StartNew();
            var result = await CreateController(context).CreateTicket(new CreateTicketDto
            {
                Subject = "Scale auto assign",
                Description = "Opis",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET
            });
            stopwatch.Stop();

            result.Should().BeOfType<CreatedAtActionResult>();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxAutoAssignTimeMilliseconds,
                because: $"scale auto-assign mora ostati ispod 3s; mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
