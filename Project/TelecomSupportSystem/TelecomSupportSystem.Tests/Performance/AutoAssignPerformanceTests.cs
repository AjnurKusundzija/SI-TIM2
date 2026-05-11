using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Performance
{
    // US-25 / NFR-04: Performansno testiranje automatske dodjele tiketa.
    // Cijeli tok (create + lookup tima + lookup agenata + dodjela) mora biti < 3 sekunde.
    public class AutoAssignPerformanceTests
    {
        private const int MaxAutoAssignTimeMilliseconds = 3000;

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId)
        {
            var controller = new TicketController(new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context)));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, "CLIENT"),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };
            return controller;
        }

        // US-25 / NFR-04: cijeli auto-assign tok (uključujući query agenata s opterećenjem) je < 3s pri realističnom broju agenata
        [Fact]
        public async Task CreateTicketWithAutoAssign_ShouldCompleteWithinThreeSeconds_AtScale()
        {
            using var context = CreateDbContext();

            // Tim + 20 dostupnih agenata, svaki s različitim brojem postojećih dodjela (do 10)
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });

            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Klijent",
                LastName = "Test",
                Email = "c@test.ba",
                Username = "c",
                PasswordHash = "h",
                Role = Role.CLIENT,
                AccountStatus = AccountStatus.ACTIVE,
            });

            for (var i = 0; i < 20; i++)
            {
                var agentId = 100 + i;
                context.Users.Add(new User
                {
                    UserId = agentId,
                    FirstName = $"Agent{agentId}",
                    LastName = "Test",
                    Email = $"a{agentId}@test.ba",
                    Username = $"a{agentId}",
                    PasswordHash = "h",
                    Role = Role.AGENT,
                    AccountStatus = AccountStatus.ACTIVE,
                    AvailabilityStatus = AvailabilityStatus.AVAILABLE,
                    TeamId = 1,
                });
            }
            await context.SaveChangesAsync();

            // Svaki agent dobija nekoliko postojećih dodjela (variraju 0..10) — ovo opterećuje load-sort
            for (var i = 0; i < 20; i++)
            {
                var agentId = 100 + i;
                for (var k = 0; k < i % 11; k++)
                {
                    var existing = new Ticket
                    {
                        Title = $"Existing {agentId}-{k}",
                        Description = "x",
                        CreatorId = 1,
                        Status = TicketStatus.OPEN,
                        Priority = (Priority)((k % 3) + 1),
                        ProblemCategory = ProblemCategory.INTERNET,
                        CreatedDate = DateTime.UtcNow,
                    };
                    context.Tickets.Add(existing);
                    await context.SaveChangesAsync();
                    context.Set<TicketUser>().Add(new TicketUser
                    {
                        TicketId = existing.TicketId,
                        UserId = agentId,
                        TeamId = 1,
                        AssignmentDate = DateTime.UtcNow,
                        AssignmentType = AssignmentType.MANUAL,
                        Note = "",
                    });
                }
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Test",
                Description = "Opis",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET,
            });
            stopwatch.Stop();

            result.Should().BeOfType<CreatedAtActionResult>();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxAutoAssignTimeMilliseconds,
                because: $"auto-dodjela uključujući load balancing mora biti < 3s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
