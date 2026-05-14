using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.System
{
    public class Sprint7UserStoriesSystemTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateTicketController(ApplicationDbContext context, int userId, string role)
        {
            var controller = new TicketController(new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context)));

            SetUser(controller, userId, role);
            return controller;
        }

        private static CommentController CreateCommentController(ApplicationDbContext context, int userId, string role)
        {
            var controller = new CommentController(new CommentService(
                new CommentRepository(context),
                new TicketRepository(context)));

            SetUser(controller, userId, role);
            return controller;
        }

        private static void SetUser(ControllerBase controller, int userId, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
                }
            };
        }

        private static User MakeUser(
            int id,
            Role role,
            int? teamId = null,
            AvailabilityStatus? availability = null,
            Location location = Location.SARAJEVO) => new()
        {
            UserId = id,
            FirstName = $"{role}{id}",
            LastName = "Test",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            Role = role,
            AccountStatus = AccountStatus.ACTIVE,
            AvailabilityStatus = availability,
            TeamId = teamId,
            Location = location
        };

        private static Team MakeTeam(int id, TeamType type = TeamType.AGENTS) => new()
        {
            TeamId = id,
            TeamName = $"Team {id}",
            TeamType = type,
            TeamStatus = TeamStatus.ACTIVE,
            SpecializedCategory = ProblemCategory.INTERNET
        };

        private static Ticket MakeTicket(int id, int creatorId, TicketStatus status = TicketStatus.OPEN) => new()
        {
            TicketId = id,
            Title = $"Ticket {id}",
            Description = "Opis problema",
            CreatorId = creatorId,
            CreatedDate = DateTime.UtcNow.AddMinutes(-id),
            Status = status,
            Priority = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET
        };

        private static TicketUser MakeAssignment(int ticketId, int userId, int teamId, AssignmentType type = AssignmentType.MANUAL) => new()
        {
            TicketId = ticketId,
            UserId = userId,
            TeamId = teamId,
            AssignmentDate = DateTime.UtcNow,
            AssignmentType = type,
            Note = "System test assignment"
        };

        private static Mock<IHubContext<ChatHub>> CreateHubMock()
        {
            var hubMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();

            hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock
                .Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return hubMock;
        }

        [Fact]
        public async Task PB25_ClosureWorkflow_RequestAcceptAndAlreadyClosedValidation_WorksThroughControllerServiceRepo()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.CLIENT), MakeUser(2, Role.AGENT, teamId: 10));
            context.Teams.Add(MakeTeam(10));
            context.Tickets.Add(MakeTicket(100, creatorId: 1));
            context.Set<TicketUser>().Add(MakeAssignment(100, userId: 2, teamId: 10));
            await context.SaveChangesAsync();

            var agentController = CreateTicketController(context, 2, "AGENT");
            (await agentController.RequestClosure(100)).Should().BeOfType<OkObjectResult>();

            var requested = await context.Tickets.FindAsync(100);
            requested!.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            requested.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);
            requested.ClosureRequestedById.Should().Be(2);

            var clientController = CreateTicketController(context, 1, "CLIENT");
            (await clientController.AcceptClosure(100)).Should().BeOfType<OkObjectResult>();

            var closed = await context.Tickets.FindAsync(100);
            closed!.Status.Should().Be(TicketStatus.CLOSED);
            closed.ClosureRequestStatus.Should().Be(ClosureRequestStatus.ACCEPTED);
            closed.ClosedDate.Should().NotBeNull();

            (await clientController.CloseTicket(100)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PB28_InternalPriority_IsVisibleToStaffAndHiddenFromClient()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(MakeUser(1, Role.CLIENT), MakeUser(2, Role.AGENT, teamId: 10));
            context.Teams.Add(MakeTeam(10));
            context.Tickets.Add(MakeTicket(101, creatorId: 1));
            await context.SaveChangesAsync();

            var agentController = CreateTicketController(context, 2, "AGENT");
            var update = await agentController.UpdateInternalPriority(
                101,
                new UpdateInternalPriorityDto { Priority = InternalPriority.CRITICAL });

            update.Should().BeOfType<OkObjectResult>();

            var agentDetail = await agentController.GetTicketById(101);
            var staffDto = agentDetail.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<TicketDetailDto>().Subject;
            staffDto.InternalPriority.Should().Be("CRITICAL");

            var clientController = CreateTicketController(context, 1, "CLIENT");
            var clientDetail = await clientController.GetTicketById(101);
            var clientDto = clientDetail.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<TicketDetailDto>().Subject;
            clientDto.InternalPriority.Should().BeNull();
        }

        [Fact]
        public async Task PB30_AutoAssignedTicket_IsVisibleInAssignedListAndTicketDetail()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam(10));
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.AGENT, teamId: 10, availability: AvailabilityStatus.AVAILABLE));
            await context.SaveChangesAsync();

            var clientController = CreateTicketController(context, 1, "CLIENT");
            var create = await clientController.CreateTicket(new CreateTicketDto
            {
                Subject = "Internet outage",
                Description = "Nema konekcije",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET
            });

            var createdDto = create.Should().BeOfType<CreatedAtActionResult>().Subject.Value
                .Should().BeAssignableTo<GetTicketDto>().Subject;
            createdDto.AssignedAgentName.Should().Be("AGENT2 Test");

            var agentController = CreateTicketController(context, 2, "AGENT");
            var assignedResult = await agentController.GetAllTickets(assignedOnly: true);
            var assigned = assignedResult.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            assigned.Should().ContainSingle(t => t.TicketId == createdDto.TicketId);

            var detail = await agentController.GetTicketById(createdDto.TicketId);
            var detailDto = detail.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<TicketDetailDto>().Subject;
            detailDto.AssignedAgentName.Should().Be("AGENT2 Test");
        }

        [Fact]
        public async Task PB31_ManualForward_UsesSortedAvailableAgentScoresAndPersistsNewOwner()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam(10));
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.AGENT, teamId: 10, availability: AvailabilityStatus.AVAILABLE),
                MakeUser(3, Role.AGENT, teamId: 10, availability: AvailabilityStatus.AVAILABLE),
                MakeUser(4, Role.AGENT, teamId: 10, availability: AvailabilityStatus.BUSY));
            context.Tickets.Add(MakeTicket(102, creatorId: 1));
            context.Set<TicketUser>().Add(MakeAssignment(102, userId: 2, teamId: 10));
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, 2, "AGENT");
            var scoresResult = await controller.GetAgentScores(102);
            var scores = scoresResult.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<IEnumerable<AgentScoreDto>>().Subject.ToList();

            scores.Should().ContainSingle();
            scores[0].UserId.Should().Be(3);

            var forwardResult = await controller.ForwardTicketToAgent(102, new ForwardToAgentDto { TargetAgentId = 3 });
            forwardResult.Should().BeOfType<OkObjectResult>();

            var latest = context.Set<TicketUser>()
                .Where(a => a.TicketId == 102)
                .OrderByDescending(a => a.AssignmentDate)
                .First();

            latest.UserId.Should().Be(3);
            latest.AssignmentType.Should().Be(AssignmentType.FORWARDED_TO_AGENT);
        }

        [Fact]
        public async Task PB37_TechnicianCanOpenOnlyAssignedTicketDetails()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam(20, TeamType.TECHNICIANS));
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(5, Role.TECHNICIAN, teamId: 20));
            context.Tickets.AddRange(
                MakeTicket(103, creatorId: 1),
                MakeTicket(104, creatorId: 1));
            context.Set<TicketUser>().Add(MakeAssignment(103, userId: 5, teamId: 20, AssignmentType.FORWARDED_TO_TECHNICIAN));
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, 5, "TECHNICIAN");

            var assignedDetail = await controller.GetTicketById(103);
            var dto = assignedDetail.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<TicketDetailDto>().Subject;
            dto.TicketId.Should().Be(103);
            dto.Title.Should().Be("Ticket 103");
            dto.AssignedAgentName.Should().Be("TECHNICIAN5 Test");

            (await controller.GetTicketById(104)).Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task PB37_TechnicianCanCommentOnAssignedTicketButNotUnassignedOrEmptyMessage()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam(20, TeamType.TECHNICIANS));
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(5, Role.TECHNICIAN, teamId: 20));
            context.Tickets.AddRange(
                MakeTicket(105, creatorId: 1),
                MakeTicket(106, creatorId: 1));
            context.Set<TicketUser>().Add(MakeAssignment(105, userId: 5, teamId: 20, AssignmentType.FORWARDED_TO_TECHNICIAN));
            await context.SaveChangesAsync();

            var controller = CreateCommentController(context, 5, "TECHNICIAN");
            var hub = CreateHubMock();

            var commentResult = await controller.AddComment(
                105,
                new CommentController.CreateCommentRequest { Content = "Provjera na terenu je u toku." },
                hub.Object);

            var dto = commentResult.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<CommentDto>().Subject;
            dto.AuthorRole.Should().Be("TECHNICIAN");

            (await controller.AddComment(
                105,
                new CommentController.CreateCommentRequest { Content = " " },
                hub.Object)).Should().BeOfType<BadRequestObjectResult>();

            (await controller.AddComment(
                106,
                new CommentController.CreateCommentRequest { Content = "Neovlasten pristup" },
                hub.Object)).Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task PB48_AgentOpenAndClosedAssignedEndpointsReturnOnlyCurrentLatestAssignments()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam(10));
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.AGENT, teamId: 10, availability: AvailabilityStatus.AVAILABLE),
                MakeUser(3, Role.AGENT, teamId: 10, availability: AvailabilityStatus.AVAILABLE));
            context.Tickets.AddRange(
                MakeTicket(107, creatorId: 1, status: TicketStatus.OPEN),
                MakeTicket(108, creatorId: 1, status: TicketStatus.CLOSED),
                MakeTicket(109, creatorId: 1, status: TicketStatus.OPEN),
                MakeTicket(110, creatorId: 1, status: TicketStatus.CLOSED));
            context.Set<TicketUser>().AddRange(
                MakeAssignment(107, userId: 2, teamId: 10),
                MakeAssignment(108, userId: 2, teamId: 10),
                MakeAssignment(109, userId: 3, teamId: 10),
                MakeAssignment(110, userId: 3, teamId: 10));
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, 2, "AGENT");

            var openResult = await controller.GetOpenAssignedTickets();
            var openTickets = openResult.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            openTickets.Should().ContainSingle(t => t.TicketId == 107);
            openTickets.Should().NotContain(t => t.Status == "CLOSED");

            var closedResult = await controller.GetClosedAssignedTickets();
            var closedTickets = closedResult.Should().BeOfType<OkObjectResult>().Subject.Value
                .Should().BeAssignableTo<IEnumerable<MyTicketDto>>().Subject.ToList();
            closedTickets.Should().ContainSingle(t => t.TicketId == 108);
            closedTickets.Should().OnlyContain(t => t.Status == "CLOSED");
        }
    }
}
