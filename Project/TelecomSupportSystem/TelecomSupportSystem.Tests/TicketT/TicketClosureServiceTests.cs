using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;
using NotificationType = TelecomSupportSystem.DAL.Entities.Enums.NotificationType;

namespace TelecomSupportSystem.Tests.TicketT
{
    public class TicketClosureServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
        private readonly Mock<ITeamRepository> _teamRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<ICommentService> _commentServiceMock = new();
        private readonly TicketService _ticketService;

        private const int TicketId = 30;
        private const int ClientId = 10;
        private const int AgentId = 20;
        private const int TechnicianId = 21;
        private const int OtherAgentId = 22;

        public TicketClosureServiceTests()
        {
            _ticketService = new TicketService(
                _ticketRepositoryMock.Object,
                _teamRepositoryMock.Object,
                _userRepositoryMock.Object,
                _notificationServiceMock.Object,
                _commentServiceMock.Object);
        }

        private static User MakeUser(int userId, Role role, string firstName) => new()
        {
            UserId = userId,
            FirstName = firstName,
            LastName = "Test",
            Role = role,
            AccountStatus = AccountStatus.ACTIVE
        };

        private static Ticket MakeClosureRequestedTicket() => new()
        {
            TicketId = TicketId,
            Title = "Test tiket",
            Description = "Opis",
            CreatorId = ClientId,
            Creator = MakeUser(ClientId, Role.CLIENT, "Client"),
            Status = TicketStatus.CLOSURE_REQUESTED,
            Priority = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = DateTime.Now.AddDays(-8),
            ClosureRequestStatus = ClosureRequestStatus.PENDING,
            Assignments = new List<TicketUser>()
        };

        [Fact]
        public async Task AcceptClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientAcceptsClosure()
        {
            var ticket = MakeClosureRequestedTicket();
            var agent = MakeUser(AgentId, Role.AGENT, "Agent");
            var technician = MakeUser(TechnicianId, Role.TECHNICIAN, "Technician");
            var baseDate = DateTime.UtcNow.AddMinutes(-10);

            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = TicketId,
                UserId = AgentId,
                User = agent,
                AssignmentDate = baseDate,
                AssignmentType = AssignmentType.AUTOMATIC
            });
            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 2,
                TicketId = TicketId,
                UserId = TechnicianId,
                User = technician,
                AssignmentDate = baseDate.AddMinutes(1),
                AssignmentType = AssignmentType.FORWARDED_TO_TECHNICIAN
            });

            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.AcceptClosureAsync(TicketId, ClientId);

            ticket.Status.Should().Be(TicketStatus.CLOSED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.ACCEPTED);
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                AgentId,
                "Tiket zatvoren",
                It.Is<string>(message => message.Contains("prihvatio zatvaranje")),
                NotificationType.TICKET_CLOSED,
                TicketId), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                TechnicianId,
                "Tiket zatvoren",
                It.Is<string>(message => message.Contains("prihvatio zatvaranje")),
                NotificationType.TICKET_CLOSED,
                TicketId), Times.Once);
        }

        [Fact]
        public async Task RejectClosureAsync_ShouldNotifyAllActiveAssignedStaff_WhenClientRejectsClosure()
        {
            var ticket = MakeClosureRequestedTicket();
            var agent = MakeUser(AgentId, Role.AGENT, "Agent");
            var technician = MakeUser(TechnicianId, Role.TECHNICIAN, "Technician");
            var baseDate = DateTime.UtcNow.AddMinutes(-10);

            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = TicketId,
                UserId = AgentId,
                User = agent,
                AssignmentDate = baseDate,
                AssignmentType = AssignmentType.AUTOMATIC
            });
            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 2,
                TicketId = TicketId,
                UserId = TechnicianId,
                User = technician,
                AssignmentDate = baseDate.AddMinutes(1),
                AssignmentType = AssignmentType.FORWARDED_TO_TECHNICIAN
            });

            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.RejectClosureAsync(TicketId, ClientId);

            ticket.Status.Should().Be(TicketStatus.OPEN);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.REJECTED);
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                AgentId,
                "Zatvaranje odbijeno",
                It.Is<string>(message => message.Contains("odbio zatvaranje")),
                NotificationType.STATUS_CHANGED,
                TicketId), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                TechnicianId,
                "Zatvaranje odbijeno",
                It.Is<string>(message => message.Contains("odbio zatvaranje")),
                NotificationType.STATUS_CHANGED,
                TicketId), Times.Once);
        }

        [Fact]
        public async Task ForceCloseAsync_ShouldNotifyClient_WhenAssignedStaffForceClosesTicket()
        {
            var ticket = MakeClosureRequestedTicket();
            var agent = MakeUser(AgentId, Role.AGENT, "Agent");
            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = TicketId,
                UserId = AgentId,
                User = agent,
                AssignmentDate = DateTime.UtcNow.AddDays(-8),
                AssignmentType = AssignmentType.AUTOMATIC
            });

            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.ForceCloseAsync(TicketId, AgentId, "AGENT");

            ticket.Status.Should().Be(TicketStatus.CLOSED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.EXPIRED);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                ClientId,
                "Tiket zatvoren",
                It.Is<string>(message => message.Contains(ticket.Title)),
                NotificationType.TICKET_CLOSED,
                TicketId), Times.Once);
        }

        [Fact]
        public async Task ForceCloseAsync_ShouldThrowUnauthorized_WhenStaffIsNotAssigned()
        {
            var ticket = MakeClosureRequestedTicket();
            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = TicketId,
                UserId = AgentId,
                User = MakeUser(AgentId, Role.AGENT, "Agent"),
                AssignmentDate = DateTime.UtcNow.AddDays(-8),
                AssignmentType = AssignmentType.AUTOMATIC
            });

            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            var act = () => _ticketService.ForceCloseAsync(TicketId, OtherAgentId, "AGENT");

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>(),
                It.IsAny<int?>()), Times.Never);
        }
    }
}
