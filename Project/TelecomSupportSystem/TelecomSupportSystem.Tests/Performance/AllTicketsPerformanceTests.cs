using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Performance
{
    // PB-32, PB-33: Performansno testiranje pregleda i pretrage svih tiketa
    public class AllTicketsPerformanceTests
    {
        private const int MaxLoadTimeMilliseconds = 2000;

        // PB-32: lista od 500 tiketa se ucitava u prihvatljivom vremenu
        [Fact]
        public async Task GetAllTickets_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Agent",
                LastName = "Test",
                Email = "agent@test.ba",
                Username = "agent",
                PasswordHash = "hash",
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.AGENT,
            });
            for (var i = 1; i <= 500; i++)
            {
                context.Tickets.Add(new Ticket
                {
                    Title = $"Tiket {i}",
                    Description = "Opis.",
                    CreatorId = 1,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.MEDIUM,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate = DateTime.UtcNow.AddMinutes(-i),
                });
            }
            await context.SaveChangesAsync();

            var controller = new TicketController(new TicketService(new TicketRepository(context), new TeamRepository(context), new UserRepository(context), new Mock<INotificationService>().Object, new Mock<ICommentService>().Object));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Role, "AGENT"),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.GetAllTickets(assignedOnly: false);
            stopwatch.Stop();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject;
            tickets.Should().HaveCount(500);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxLoadTimeMilliseconds,
                because: $"500 tiketa mora se ucitati u < 2s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
