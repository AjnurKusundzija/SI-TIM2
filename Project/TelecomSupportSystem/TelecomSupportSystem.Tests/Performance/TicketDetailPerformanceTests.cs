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

namespace TelecomSupportSystem.Tests.Performance
{
    // PB-24: Performansno testiranje prikaza detalja tiketa
    public class TicketDetailPerformanceTests
    {
        private const int MaxLoadTimeMilliseconds = 2000;

        // US-14: detalji tiketa (s includeom Creator i Assignments) se ucitavaju u prihvatljivom vremenu
        [Fact]
        public async Task GetTicketById_ShouldLoadWithinTimeLimit_InTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var client = new User
            {
                UserId = 1,
                FirstName = "Merjem",
                LastName = "Omerovic",
                Email = "m@test.ba",
                Username = "merjem",
                PasswordHash = "hash",
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.CLIENT,
            };
            context.Users.Add(client);
            context.Tickets.Add(new Ticket
            {
                TicketId = 1,
                Title = "Test tiket",
                Description = "Opis.",
                CreatorId = 1,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.HIGH,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>(),
            });
            await context.SaveChangesAsync();

            var controller = new TicketController(new TicketService(new TicketRepository(context)));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Role, "CLIENT"),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.GetTicketById(1);
            stopwatch.Stop();

            result.Should().BeOfType<OkObjectResult>();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxLoadTimeMilliseconds,
                because: $"detalji tiketa moraju se ucitati u < 2s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
