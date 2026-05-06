using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Performance
{
    // PB-22, PB-23: Performansno testiranje kreiranja i pregleda tiketa
    public class TicketPerformanceTests
    {
        private const int MaxCreateTimeMilliseconds = 3000;
        private const int MaxListTimeMilliseconds = 2000;

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role = "CLIENT")
        {
            var controller = new TicketController(new TicketService(new TicketRepository(context)));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };
            return controller;
        }

        // PB-22 / NFR-04: kreiranje tiketa se mora izvrsiti u manje od 3 sekunde
        [Fact]
        public async Task CreateTicket_ShouldCompleteWithinThreeSeconds_InTestEnvironment()
        {
            using var context = CreateDbContext();
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "User",
                LastName = "One",
                Email = "u1@test.ba",
                Username = "u1",
                PasswordHash = "hash",
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.CLIENT,
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Test tiket",
                Description = "Opis.",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET,
            });
            stopwatch.Stop();

            result.Should().BeOfType<CreatedAtActionResult>();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxCreateTimeMilliseconds,
                because: $"kreiranje tiketa mora biti u < 3s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }

        // PB-23: lista od 200 tiketa se ucitava u prihvatljivom vremenu
        [Fact]
        public async Task GetMyTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment()
        {
            using var context = CreateDbContext();
            context.Users.Add(new User
            {
                UserId = 2,
                FirstName = "User",
                LastName = "Two",
                Email = "u2@test.ba",
                Username = "u2",
                PasswordHash = "hash",
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.CLIENT,
            });
            for (var i = 1; i <= 200; i++)
            {
                context.Tickets.Add(new Ticket
                {
                    Title = $"Tiket {i}",
                    Description = "Opis.",
                    CreatorId = 2,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.MEDIUM,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-i),
                });
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2);

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.GetMyTickets();
            stopwatch.Stop();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject;
            tickets.Should().HaveCount(200);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxListTimeMilliseconds,
                because: $"lista od 200 tiketa mora se ucitati u < 2s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
