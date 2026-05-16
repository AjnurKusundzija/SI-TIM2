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
    // PB-24: Integracijsko testiranje prikaza detalja tiketa kroz sve slojeve
    public class TicketDetailIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role)
        {
            var repo = new TicketRepository(context);
            var service = new TicketService(repo, new TeamRepository(context), new UserRepository(context), new Mock<INotificationService>().Object, new Mock<ICommentService>().Object);
            var controller = new TicketController(service);

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

        private static User MakeUser(int id, string first, string last, Role role = Role.CLIENT) => new()
        {
            UserId = id,
            FirstName = first,
            LastName = last,
            Email = $"user{id}@test.ba",
            Username = $"user{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = role,
        };

        // US-14: klijent vidi vlastiti tiket i sva polja su ispravno mapirana
        [Fact]
        public async Task GetTicketById_ClientOwner_ReturnsDetailDtoWithAllFields()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, "Merjem", "Omerovic");
            context.Users.Add(client);
            context.Tickets.Add(new Ticket
            {
                TicketId = 10,
                Title = "Internet ne radi",
                Description = "Nema veze od jutros.",
                CreatorId = 1,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.HIGH,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = new DateTime(2026, 5, 1, 10, 0, 0),
                Assignments = new List<TicketUser>(),
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetTicketById(10);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeAssignableTo<TicketDetailDto>().Subject;
            dto.Title.Should().Be("Internet ne radi");
            dto.Description.Should().Be("Nema veze od jutros.");
            dto.Status.Should().Be("OPEN");
            dto.ClientName.Should().Be("Merjem Omerovic");
        }

        // US-30: agent vidi svaki tiket bez obzira na vlasnistvo
        [Fact]
        public async Task GetTicketById_AgentRole_CanAccessAnyTicket()
        {
            using var context = CreateDbContext();
            var client = MakeUser(2, "Amir", "Hadzic");
            var agent = MakeUser(99, "Selma", "Mujic", Role.AGENT);
            context.Users.AddRange(client, agent);
            context.Tickets.Add(new Ticket
            {
                TicketId = 20,
                Title = "TV problem",
                Description = "Signal nestaje.",
                CreatorId = 2,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.TV,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>(),
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 99, role: "AGENT");
            var result = await controller.GetTicketById(20);

            result.Should().BeOfType<OkObjectResult>();
        }

        // US-14: klijent ne moze vidjeti tudji tiket — controller vraca 403 Forbid
        [Fact]
        public async Task GetTicketById_ClientAccessingOtherTicket_ReturnsForbid()
        {
            using var context = CreateDbContext();
            var owner = MakeUser(3, "Jasmin", "Basic");
            var intruder = MakeUser(4, "Vedran", "Kovac");
            context.Users.AddRange(owner, intruder);
            context.Tickets.Add(new Ticket
            {
                TicketId = 30,
                Title = "Racun netacan",
                Description = "Iznos je pogresno izracunat.",
                CreatorId = 3,
                Creator = owner,
                Status = TicketStatus.OPEN,
                Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.BILLING,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>(),
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 4, role: "CLIENT");
            var result = await controller.GetTicketById(30);

            result.Should().BeOfType<ForbidResult>();
        }

        // US-14: nepostojeci tiket vraca 404 NotFound
        [Fact]
        public async Task GetTicketById_TicketDoesNotExist_ReturnsNotFound()
        {
            using var context = CreateDbContext();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetTicketById(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
