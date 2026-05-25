using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-45 / US-71 (NFR) — Dashboard mora učitati metrike u manje od 5 sekundi
    // za tipičan dataset (200 tiketa, 5 agenata, klijent populacija).
    public class AdminDashboardPerformanceTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        [Fact]
        public async Task GetDashboard_ShouldCompleteWithinFiveSeconds_ForTypicalDataset()
        {
            using var context = CreateDbContext();
            context.Users.Add(new User
            {
                UserId = 1, FirstName = "A", LastName = "A", Email = "a@t", Username = "a", PasswordHash = "h",
                Role = Role.ADMINISTRATOR, AccountStatus = AccountStatus.ACTIVE,
            });

            var rng = new Random(42);
            for (int i = 2; i <= 6; i++)
            {
                context.Users.Add(new User
                {
                    UserId = i, FirstName = $"Ag{i}", LastName = "X", Email = $"ag{i}@t", Username = $"ag{i}",
                    PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE,
                });
            }
            for (int i = 7; i <= 50; i++)
            {
                context.Users.Add(new User
                {
                    UserId = i, FirstName = $"Cl{i}", LastName = "X", Email = $"cl{i}@t", Username = $"cl{i}",
                    PasswordHash = "h", Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE,
                });
            }

            var now = DateTime.UtcNow;
            for (int i = 1; i <= 200; i++)
            {
                var status = (TicketStatus)(rng.Next(0, 3) + 1);
                var t = new Ticket
                {
                    TicketId = i, Title = $"T{i}", Description = "D", CreatorId = 7 + (i % 40),
                    Status = status, Priority = Priority.MEDIUM,
                    ProblemCategory = (ProblemCategory)(rng.Next(0, 5) + 1),
                    CreatedDate = now.AddDays(-rng.Next(1, 28)),
                    ClosedDate = status == TicketStatus.CLOSED ? now.AddDays(-1) : null,
                };
                context.Tickets.Add(t);
            }
            await context.SaveChangesAsync();

            var service = new ReportService(new ReportRepository(context));
            var controller = new AdminController(service);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "ADMINISTRATOR"),
                    }, "Test"))
                }
            };

            var sw = Stopwatch.StartNew();
            var result = await controller.GetDashboard("month", null, null);
            sw.Stop();

            result.Should().BeOfType<OkObjectResult>();
            sw.ElapsedMilliseconds.Should().BeLessThan(5_000,
                "PB-45 NFR — dashboard < 5s na tipičnom datasetu");
        }
    }
}
