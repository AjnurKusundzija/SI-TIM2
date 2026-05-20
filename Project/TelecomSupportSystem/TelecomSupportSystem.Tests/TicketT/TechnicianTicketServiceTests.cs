using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.TicketT
{
    /// <summary>
    /// US-35 / US-36 / US-37 / US-38 – Technician ticket workflow unit tests.
    /// Tests GetAllTicketsAsync (technician view) and UpdateTicketStatusAsync authorization.
    /// </summary>
    public class TechnicianTicketServiceTests
    {
        private readonly Mock<ITicketRepository>    _ticketRepoMock      = new();
        private readonly Mock<ITeamRepository>      _teamRepoMock        = new();
        private readonly Mock<IUserRepository>      _userRepoMock        = new();
        private readonly Mock<INotificationService> _notificationMock    = new();
        private readonly Mock<ICommentService>      _commentServiceMock  = new();
        private readonly TicketService              _service;

        private const int TechId    = 5;
        private const int OtherTech = 6;
        private const int ClientId  = 10;
        private const int AgentId   = 20;

        public TechnicianTicketServiceTests()
        {
            _service = new TicketService(
                _ticketRepoMock.Object,
                _teamRepoMock.Object,
                _userRepoMock.Object,
                _notificationMock.Object,
                _commentServiceMock.Object);
        }

        private static User MakeUser(int id, Role role) => new()
        {
            UserId        = id,
            FirstName     = "Test",
            LastName      = "User",
            Role          = role,
            AccountStatus = AccountStatus.ACTIVE,
        };

        private static Ticket MakeAssignedTicket(int id, int assigneeId, TicketStatus status = TicketStatus.OPEN) => new()
        {
            TicketId        = id,
            Title           = $"Ticket {id}",
            Description     = "Issue",
            CreatorId       = ClientId,
            Status          = status,
            Priority        = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow.AddDays(-1),
            Assignments     = new List<TicketUser>
            {
                new()
                {
                    AssignmentId   = id,
                    TicketId       = id,
                    UserId         = assigneeId,
                    AssignmentDate = DateTime.UtcNow.AddDays(-1),
                    AssignmentType = AssignmentType.FORWARDED_TO_TECHNICIAN,
                    User           = MakeUser(assigneeId, Role.TECHNICIAN),
                }
            },
        };

        // ─── Technician sees only assigned tickets ────────────────────────────

        /// <summary>
        /// US-35: Technician's GetAllTicketsAsync returns only tickets assigned to them.
        /// </summary>
        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnOnlyAssignedTickets_WhenRoleIsTechnician()
        {
            var assignedToTech = new[]
            {
                MakeAssignedTicket(1, TechId),
                MakeAssignedTicket(2, TechId),
            };
            _ticketRepoMock.Setup(r => r.GetByAssigneeIdAsync(TechId))
                .ReturnsAsync(assignedToTech);

            var result = await _service.GetAllTicketsAsync(TechId, "TECHNICIAN");

            var list = result.ToList();
            list.Should().HaveCount(2);
        }

        /// <summary>
        /// US-35: Technician with no assignments gets empty list (not 403).
        /// </summary>
        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnEmpty_WhenTechnicianHasNoAssignedTickets()
        {
            _ticketRepoMock.Setup(r => r.GetByAssigneeIdAsync(TechId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());

            var result = await _service.GetAllTicketsAsync(TechId, "TECHNICIAN");

            result.Should().BeEmpty();
        }

        /// <summary>
        /// US-35: CLIENT role cannot call GetAllTicketsAsync — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task GetAllTicketsAsync_ShouldThrowUnauthorized_WhenRoleIsClient()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetAllTicketsAsync(ClientId, "CLIENT"));
        }

        // ─── Technician can update status of assigned ticket ──────────────────

        /// <summary>
        /// US-37: Technician can set status CLOSURE_REQUESTED on assigned ticket.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldSetClosureRequested_WhenTechnicianUpdatesAssignedTicket()
        {
            var ticket = MakeAssignedTicket(10, TechId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(10)).ReturnsAsync(ticket);

            await _service.UpdateTicketStatusAsync(10, TicketStatus.CLOSURE_REQUESTED, TechId, "TECHNICIAN");

            ticket.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-37: Technician can reopen a CLOSURE_REQUESTED ticket back to OPEN.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldSetOpen_WhenTechnicianCancelsClosureRequest()
        {
            var ticket = MakeAssignedTicket(11, TechId, TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestStatus = ClosureRequestStatus.PENDING;
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(11)).ReturnsAsync(ticket);

            await _service.UpdateTicketStatusAsync(11, TicketStatus.OPEN, TechId, "TECHNICIAN");

            ticket.Status.Should().Be(TicketStatus.OPEN);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.REJECTED);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        // ─── Technician cannot update status of unassigned ticket ─────────────

        /// <summary>
        /// US-38: Technician cannot update status of a ticket assigned to another technician.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenTicketNotAssignedToTechnician()
        {
            var ticket = MakeAssignedTicket(20, assigneeId: OtherTech);  // assigned to OtherTech
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(20)).ReturnsAsync(ticket);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateTicketStatusAsync(20, TicketStatus.CLOSURE_REQUESTED, TechId, "TECHNICIAN"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-38: Non-technician role (AGENT) cannot use UpdateTicketStatusAsync.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenRoleIsAgent()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateTicketStatusAsync(1, TicketStatus.CLOSURE_REQUESTED, AgentId, "AGENT"));
        }

        /// <summary>
        /// US-38: Status update on an already CLOSED ticket throws InvalidOperationException.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTicketIsClosed()
        {
            var ticket = MakeAssignedTicket(30, TechId, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(30)).ReturnsAsync(ticket);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateTicketStatusAsync(30, TicketStatus.OPEN, TechId, "TECHNICIAN"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-37: Technician cannot set CLOSED status directly — only allowed statuses are OPEN and CLOSURE_REQUESTED.
        /// </summary>
        [Theory]
        [InlineData(TicketStatus.OPEN)]
        [InlineData(TicketStatus.CLOSURE_REQUESTED)]
        public async Task UpdateTicketStatusAsync_ShouldAcceptAllowedStatuses_WhenTechnicianUpdates(TicketStatus status)
        {
            var ticket = MakeAssignedTicket(40, TechId,
                status == TicketStatus.CLOSURE_REQUESTED ? TicketStatus.OPEN : TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestStatus = status == TicketStatus.OPEN
                ? ClosureRequestStatus.PENDING
                : null;
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(40)).ReturnsAsync(ticket);

            var exception = await Record.ExceptionAsync(() =>
                _service.UpdateTicketStatusAsync(40, status, TechId, "TECHNICIAN"));

            exception.Should().BeNull();
        }

        /// <summary>
        /// US-37: Technician cannot set CLOSED status directly — InvalidOperationException.
        /// </summary>
        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTechnicianTriesToSetClosed()
        {
            var ticket = MakeAssignedTicket(50, TechId, TicketStatus.OPEN);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(50)).ReturnsAsync(ticket);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateTicketStatusAsync(50, TicketStatus.CLOSED, TechId, "TECHNICIAN"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }
    }
}
