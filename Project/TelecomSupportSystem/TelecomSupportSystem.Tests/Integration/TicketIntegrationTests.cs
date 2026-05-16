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

namespace TelecomSupportSystem.Tests.Integration
{
    // PB-22, PB-23: Integracijsko testiranje kreiranja i pregleda tiketa kroz sve slojeve
    public class TicketIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role = "CLIENT")
        {
            var repo = new TicketRepository(context);
            var service = new TicketService(repo, new TeamRepository(context), new UserRepository(context), new Mock<INotificationService>().Object);
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

        private static User MakeUser(int id) => new()
        {
            UserId = id,
            FirstName = "Amir",
            LastName = "Hadzic",
            Email = $"user{id}@test.ba",
            Username = $"user{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = Role.CLIENT,
        };

        // PB-22 / US-8: kreiran tiket se sprema u bazu i vraća 201 sa ispravnim podacima
        [Fact]
        public async Task CreateTicket_PersistsToDatabase_AndReturnsCreated()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);

            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Internet ne radi",
                Description = "Nema veze od jutros.",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET,
            });

            result.Should().BeOfType<CreatedAtActionResult>();
            context.Tickets.Should().ContainSingle(t =>
                t.Title == "Internet ne radi" &&
                t.Status == TicketStatus.OPEN &&
                t.CreatorId == 1);
        }

        // PB-22 / US-9: novi tiket uvijek dobija status OPEN
        [Fact]
        public async Task CreateTicket_NewTicketAlwaysHasStatusOpen()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(2));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 2);

            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "TV signal nestaje",
                Description = "Signal pada svakih par minuta.",
                Priority = Priority.MEDIUM,
                Type = ProblemCategory.TV,
            });

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.Status.Should().Be("OPEN");
        }

        // PB-23 / US-11: korisnik vidi samo vlastite tikete — tiketi drugog korisnika se ne vraćaju
        [Fact]
        public async Task GetMyTickets_ReturnsOnlyOwnTickets_NotOtherUsersTickets()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(3), MakeUser(4));
            context.Tickets.AddRange(
                new Ticket { Title = "Moj tiket", Description = "d", CreatorId = 3, Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow },
                new Ticket { Title = "Tudji tiket", Description = "d", CreatorId = 4, Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatedDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 3);
            var result = await controller.GetMyTickets();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            tickets.Should().ContainSingle();
            tickets[0].Title.Should().Be("Moj tiket");
        }

        // PB-23 / US-12: OPEN i CLOSED statusi se ispravno mapiraju u string
        [Fact]
        public async Task GetMyTickets_MapsStatusCorrectlyToString()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(5));
            context.Tickets.AddRange(
                new Ticket { Title = "Otvoren", Description = "d", CreatorId = 5, Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow },
                new Ticket { Title = "Zatvoren", Description = "d", CreatorId = 5, Status = TicketStatus.CLOSED, Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatedDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 5);
            var result = await controller.GetMyTickets();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            tickets.Should().Contain(t => t.Status == "OPEN");
            tickets.Should().Contain(t => t.Status == "CLOSED");
        }

        // PB-23 / US-13: prazna lista kada korisnik nema tiketa
        [Fact]
        public async Task GetMyTickets_ReturnsEmptyList_WhenUserHasNoTickets()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(6));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 6);
            var result = await controller.GetMyTickets();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject;
            tickets.Should().BeEmpty();
        }
    }
}
