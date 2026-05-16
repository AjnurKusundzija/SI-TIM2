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
    // PB-32, PB-33: Integracijsko testiranje pregleda svih tiketa i filtriranja kroz sve slojeve
    public class AllTicketsIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role)
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

        private static User MakeUser(int id, Role role = Role.CLIENT) => new()
        {
            UserId = id,
            FirstName = "User",
            LastName = $"{id}",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = role,
        };

        private static Ticket MakeTicket(int id, int creatorId, ProblemCategory category = ProblemCategory.INTERNET) => new()
        {
            TicketId = id,
            Title = $"Tiket {id}",
            Description = "Opis.",
            CreatorId = creatorId,
            Status = TicketStatus.OPEN,
            Priority = Priority.MEDIUM,
            ProblemCategory = category,
            CreatedDate = DateTime.UtcNow.AddMinutes(-id),
        };

        // PB-32 / US-29: agent vidi sve tikete iz sistema
        [Fact]
        public async Task GetAllTickets_AgentRole_ReturnsAllTickets()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1), MakeUser(2), MakeUser(99, Role.AGENT));
            context.Tickets.AddRange(
                MakeTicket(1, creatorId: 1),
                MakeTicket(2, creatorId: 2),
                MakeTicket(3, creatorId: 1)
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 99, role: "AGENT");
            var result = await controller.GetAllTickets(assignedOnly: false);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject;
            tickets.Should().HaveCount(3);
        }

        // PB-32: klijent ne moze pristupiti listi svih tiketa — 403 Forbid
        [Fact]
        public async Task GetAllTickets_ClientRole_ReturnsForbid()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetAllTickets();

            result.Should().BeOfType<ForbidResult>();
        }

        // PB-32 / US-30: agent sa assignedOnly=true dobija samo tikete na kojima je dodijeljen
        [Fact]
        public async Task GetAllTickets_AgentWithAssignedOnlyTrue_ReturnsOnlyAssignedTickets()
        {
            using var context = CreateDbContext();
            var agent = MakeUser(50, Role.AGENT);
            var client = MakeUser(1);
            context.Users.AddRange(agent, client);

            var assignedTicket = MakeTicket(10, creatorId: 1);
            var unassignedTicket = MakeTicket(11, creatorId: 1);
            context.Tickets.AddRange(assignedTicket, unassignedTicket);
            await context.SaveChangesAsync();

            context.Set<TicketUser>().Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = 10,
                UserId = 50,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.MANUAL,
                TeamId = 0,
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 50, role: "AGENT");
            var result = await controller.GetAllTickets(assignedOnly: true);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            tickets.Should().ContainSingle();
            tickets[0].TicketId.Should().Be(10);
        }

        // PB-33 / US-31: tiketi su sortirani od najnovijeg prema najstarijem
        [Fact]
        public async Task GetAllTickets_ReturnsTicketsOrderedByDateDescending()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1), MakeUser(99, Role.AGENT));
            context.Tickets.AddRange(
                new Ticket { TicketId = 100, Title = "Stariji", Description = "d", CreatorId = 1, Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.TV, CreatedDate = new DateTime(2026, 4, 1) },
                new Ticket { TicketId = 101, Title = "Noviji", Description = "d", CreatorId = 1, Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = new DateTime(2026, 5, 1) }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 99, role: "AGENT");
            var result = await controller.GetAllTickets();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            tickets[0].Title.Should().Be("Noviji");
            tickets[1].Title.Should().Be("Stariji");
        }
    }
}
