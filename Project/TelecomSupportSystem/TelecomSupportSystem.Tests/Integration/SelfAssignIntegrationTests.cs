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
    // PB-62 / US-105: End-to-end testovi samodjelovanja tiketa (Controller → Service → Repository → DB).
    public class SelfAssignIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId, string role)
        {
            var service = new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object,
                new Mock<ICommentService>().Object);
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

        private static User MakeAgent(int id, int teamId) => new()
        {
            UserId = id,
            FirstName = $"Agent{id}",
            LastName = "Test",
            Email = $"a{id}@test.ba",
            Username = $"a{id}",
            PasswordHash = "h",
            Role = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            AvailabilityStatus = AvailabilityStatus.AVAILABLE,
            TeamId = teamId,
        };

        private static Ticket MakeUnassignedTicket(int id, int creatorId, int? teamId = null) => new()
        {
            TicketId = id,
            Title = "Tiket",
            Description = "Opis",
            CreatorId = creatorId,
            Status = TicketStatus.OPEN,
            Priority = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = DateTime.UtcNow,
            TeamId = teamId,
        };

        [Fact]
        public async Task SelfAssign_ShouldAssignTicket_ToCallingAgent()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            context.Users.AddRange(MakeClient(1), MakeAgent(10, 1));
            context.Tickets.Add(MakeUnassignedTicket(100, creatorId: 1, teamId: 1));
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 10, role: "AGENT");

            var result = await controller.SelfAssignTicket(100);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<AgentScoreDto>().Subject;
            dto.UserId.Should().Be(10);

            context.Set<TicketUser>().Should().ContainSingle(a =>
                a.TicketId == 100
                && a.UserId == 10
                && a.AssignmentType == AssignmentType.MANUAL);
        }

        [Fact]
        public async Task SelfAssign_ShouldReturnForbid_ForNonAgents()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeClient(1));
            context.Tickets.Add(MakeUnassignedTicket(100, creatorId: 1));
            await context.SaveChangesAsync();

            var clientController = CreateController(context, userId: 1, role: "CLIENT");
            var clientResult = await clientController.SelfAssignTicket(100);
            clientResult.Should().BeOfType<ForbidResult>();

            var techController = CreateController(context, userId: 1, role: "TECHNICIAN");
            var techResult = await techController.SelfAssignTicket(100);
            techResult.Should().BeOfType<ForbidResult>();

            var adminController = CreateController(context, userId: 1, role: "ADMINISTRATOR");
            var adminResult = await adminController.SelfAssignTicket(100);
            adminResult.Should().BeOfType<ForbidResult>();

            context.Set<TicketUser>().Should().BeEmpty();
        }

        [Fact]
        public async Task SelfAssign_ShouldReturnBadRequest_WhenAlreadyAssignedToAnotherAgent()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            context.Users.AddRange(MakeClient(1), MakeAgent(10, 1), MakeAgent(11, 1));
            context.Tickets.Add(MakeUnassignedTicket(100, creatorId: 1, teamId: 1));
            context.Set<TicketUser>().Add(new TicketUser
            {
                TicketId = 100,
                UserId = 11,
                TeamId = 1,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.AUTOMATIC,
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 10, role: "AGENT");

            var result = await controller.SelfAssignTicket(100);

            result.Should().BeOfType<BadRequestObjectResult>();
            context.Set<TicketUser>().Should().ContainSingle()
                .Which.UserId.Should().Be(11);
        }

        [Fact]
        public async Task SelfAssign_ShouldReturnBadRequest_WhenTicketIsClosed()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            context.Users.AddRange(MakeClient(1), MakeAgent(10, 1));
            var closed = MakeUnassignedTicket(100, creatorId: 1, teamId: 1);
            closed.Status = TicketStatus.CLOSED;
            closed.ClosedDate = DateTime.UtcNow;
            context.Tickets.Add(closed);
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 10, role: "AGENT");

            var result = await controller.SelfAssignTicket(100);

            result.Should().BeOfType<BadRequestObjectResult>();
            context.Set<TicketUser>().Should().BeEmpty();
        }

        [Fact]
        public async Task SelfAssign_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeAgent(10, 1));
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context, userId: 10, role: "AGENT");

            var result = await controller.SelfAssignTicket(999);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
