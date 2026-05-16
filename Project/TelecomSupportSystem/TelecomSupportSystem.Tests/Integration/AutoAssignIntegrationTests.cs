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
    // US-25: Integracijsko testiranje automatske dodjele kroz Controller → Service → Repository → DB (InMemory).
    // Verifikuje end-to-end ponašanje umjesto izolovane logike — ako mapiranje, query ili persist puca, test će to uhvatiti.
    public class AutoAssignIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role = "CLIENT")
        {
            var service = new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object, new Mock<ICommentService>().Object);
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

        private static User MakeClient(int id) => new()
        {
            UserId = id,
            FirstName = "Client",
            LastName = $"{id}",
            Email = $"c{id}@test.ba",
            Username = $"c{id}",
            PasswordHash = "h",
            Role = Role.CLIENT,
            AccountStatus = AccountStatus.ACTIVE,
        };

        private static User MakeAgent(int id, int teamId, AvailabilityStatus status = AvailabilityStatus.AVAILABLE) => new()
        {
            UserId = id,
            FirstName = $"Agent{id}",
            LastName = "Test",
            Email = $"a{id}@test.ba",
            Username = $"a{id}",
            PasswordHash = "h",
            Role = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            AvailabilityStatus = status,
            TeamId = teamId,
        };

        // US-25 / AC1, AC3: tiket se kreira, dodjela se persistira, i agent ga vidi u svojoj listi
        [Fact]
        public async Task CreateTicket_AutoAssignsAndAgentSeesTicket_EndToEnd()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 100,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            context.Users.AddRange(
                MakeClient(1),
                MakeAgent(10, 100)
            );
            await context.SaveChangesAsync();

            // Klijent kreira tiket
            var clientController = CreateController(context, userId: 1);
            var createResult = await clientController.CreateTicket(new CreateTicketDto
            {
                Subject = "Internet ne radi",
                Description = "Nema veze.",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET,
            });

            var created = createResult.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.AssignedAgentName.Should().Be("Agent10 Test");
            dto.AssignmentMessage.Should().BeNull();

            // Dodjela mora postojati u bazi
            context.Set<TicketUser>().Should().ContainSingle(a =>
                a.UserId == 10 &&
                a.TeamId == 100 &&
                a.AssignmentType == AssignmentType.AUTOMATIC);

            // Agent dohvaća svoju listu dodijeljenih tiketa (AC3)
            var agentController = CreateController(context, userId: 10, role: "AGENT");
            var listResult = await agentController.GetAllTickets(assignedOnly: true);

            var ok = listResult.Should().BeOfType<OkObjectResult>().Subject;
            var tickets = ok.Value.Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            tickets.Should().ContainSingle();
            tickets[0].Title.Should().Be("Internet ne radi");
        }

        // US-25 / AC2: agent s AvailabilityStatus != AVAILABLE se preskače
        [Fact]
        public async Task CreateTicket_SkipsUnavailableAgent_AndPicksAvailableOne()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 200,
                TeamName = "TV Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.TV
            });
            context.Users.AddRange(
                MakeClient(1),
                MakeAgent(20, 200, AvailabilityStatus.UNAVAILABLE),
                MakeAgent(21, 200, AvailabilityStatus.BUSY),
                MakeAgent(22, 200, AvailabilityStatus.AVAILABLE)
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "TV signal",
                Description = "Pada.",
                Priority = Priority.MEDIUM,
                Type = ProblemCategory.TV,
            });

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.AssignedAgentName.Should().Be("Agent22 Test");

            context.Set<TicketUser>().Should().ContainSingle()
                .Which.UserId.Should().Be(22);
        }

        // US-25 / AC4: kada tim postoji ali nema dostupnih agenata, vraća se poruka i ne kreira se dodjela
        [Fact]
        public async Task CreateTicket_WhenTeamExistsButNoAvailableAgents_ReturnsMessageAndNoAssignment()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 300,
                TeamName = "Mobilni Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.MOBILE_NETWORK
            });
            context.Users.AddRange(
                MakeClient(1),
                MakeAgent(30, 300, AvailabilityStatus.UNAVAILABLE),
                MakeAgent(31, 300, AvailabilityStatus.BUSY)
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Nema mreže",
                Description = "Nema signala.",
                Priority = Priority.LOW,
                Type = ProblemCategory.MOBILE_NETWORK,
            });

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.AssignedAgentName.Should().BeNull();
            dto.AssignmentMessage.Should().Be("Nema dostupnih agenata. Tiket je označen kao Nedodijeljen.");

            context.Set<TicketUser>().Should().BeEmpty();
        }

        // US-25 / AC6: kada nema tima za kategoriju (nema definisanih pravila), poruka se vraća korisniku
        [Fact]
        public async Task CreateTicket_WhenNoTeamMatchesCategory_ReturnsNoRulesMessage()
        {
            using var context = CreateDbContext();
            // Namjerno nema timova
            context.Users.Add(MakeClient(1));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Tiket bez tima",
                Description = "...",
                Priority = Priority.LOW,
                Type = ProblemCategory.BILLING,
            });

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.TeamId.Should().BeNull();
            dto.AssignedAgentName.Should().BeNull();
            dto.AssignmentMessage.Should().Be("Nema definisanih pravila dodjele za odabranu kategoriju.");

            context.Set<TicketUser>().Should().BeEmpty();
        }

        // US-25 / AC5: bira agenta s najmanjim brojem trenutno dodijeljenih otvorenih tiketa
        [Fact]
        public async Task CreateTicket_PicksAgentWithFewestExistingAssignments_EndToEnd()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 400,
                TeamName = "Naplata Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.BILLING
            });
            context.Users.AddRange(
                MakeClient(1),
                MakeAgent(40, 400),
                MakeAgent(41, 400)
            );

            // Agent 40 već ima 3 dodijeljena tiketa, agent 41 nema nijedan
            for (var i = 1; i <= 3; i++)
            {
                var t = new Ticket
                {
                    Title = $"Existing {i}",
                    Description = "x",
                    CreatorId = 1,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.LOW,
                    ProblemCategory = ProblemCategory.BILLING,
                    CreatedDate = DateTime.UtcNow,
                };
                context.Tickets.Add(t);
                await context.SaveChangesAsync();
                context.Set<TicketUser>().Add(new TicketUser
                {
                    TicketId = t.TicketId,
                    UserId = 40,
                    TeamId = 400,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.MANUAL,
                    Note = ""
                });
            }
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 1);
            var result = await controller.CreateTicket(new CreateTicketDto
            {
                Subject = "Račun",
                Description = "Pogrešan iznos",
                Priority = Priority.HIGH,
                Type = ProblemCategory.BILLING,
            });

            var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var dto = created.Value.Should().BeAssignableTo<GetTicketDto>().Subject;
            dto.AssignedAgentName.Should().Be("Agent41 Test");

            context.Set<TicketUser>()
                .Count(a => a.UserId == 41 && a.AssignmentType == AssignmentType.AUTOMATIC)
                .Should().Be(1);
        }
    }
}
