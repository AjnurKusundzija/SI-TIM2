using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.TicketT
{
    /// <summary>
    /// US-16 / US-17 – Ticket closure workflow unit tests.
    /// Covers: RequestClosure, AcceptClosure, RejectClosure, ForceClose.
    /// </summary>
    public class TicketClosureWorkflowTests
    {
        private readonly Mock<ITicketRepository>     _ticketRepoMock        = new();
        private readonly Mock<ITeamRepository>       _teamRepoMock          = new();
        private readonly Mock<IUserRepository>       _userRepoMock          = new();
        private readonly Mock<INotificationService>  _notificationMock      = new();
        private readonly Mock<ICommentService>       _commentServiceMock    = new();
        private readonly TicketService               _service;

        private const int AgentId      = 10;
        private const int TechId       = 11;
        private const int ClientId     = 20;
        private const int OtherUserId  = 99;
        private const int TicketId     = 1;

        public TicketClosureWorkflowTests()
        {
            _service = new TicketService(
                _ticketRepoMock.Object,
                _teamRepoMock.Object,
                _userRepoMock.Object,
                _notificationMock.Object,
                _commentServiceMock.Object);
        }

        // ─── Helpers ────────────────────────────────────────────────────────────

        private static User MakeUser(int id, Role role) => new()
        {
            UserId        = id,
            FirstName     = "Test",
            LastName      = "User",
            Role          = role,
            AccountStatus = AccountStatus.ACTIVE,
        };

        private static Ticket MakeOpenTicket(int assignedAgentId) => new()
        {
            TicketId        = TicketId,
            Title           = "Internet problem",
            Description     = "No connection",
            CreatorId       = ClientId,
            Creator         = MakeUser(ClientId, Role.CLIENT),
            Status          = TicketStatus.OPEN,
            Priority        = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow.AddDays(-2),
            Comments        = new List<Comment>(),
            Assignments     = new List<TicketUser>
            {
                new()
                {
                    AssignmentId   = 1,
                    TicketId       = TicketId,
                    UserId         = assignedAgentId,
                    AssignmentDate = DateTime.UtcNow.AddDays(-2),
                    AssignmentType = AssignmentType.AUTOMATIC,
                    User           = MakeUser(assignedAgentId, Role.AGENT),
                }
            },
        };

        private static Ticket MakeClosureRequestedTicket(int assignedAgentId) => new()
        {
            TicketId              = TicketId,
            Title                 = "Internet problem",
            Description           = "No connection",
            CreatorId             = ClientId,
            Creator               = MakeUser(ClientId, Role.CLIENT),
            Status                = TicketStatus.CLOSURE_REQUESTED,
            Priority              = Priority.MEDIUM,
            ProblemCategory       = ProblemCategory.INTERNET,
            CreatedDate           = DateTime.UtcNow.AddDays(-10),
            ClosureRequestedDate  = DateTime.UtcNow.AddDays(-8),
            ClosureRequestedById  = assignedAgentId,
            ClosureRequestStatus  = ClosureRequestStatus.PENDING,
            Comments              = new List<Comment>(),
            Assignments           = new List<TicketUser>
            {
                new()
                {
                    AssignmentId   = 1,
                    TicketId       = TicketId,
                    UserId         = assignedAgentId,
                    AssignmentDate = DateTime.UtcNow.AddDays(-10),
                    AssignmentType = AssignmentType.AUTOMATIC,
                    User           = MakeUser(assignedAgentId, Role.AGENT),
                }
            },
        };

        // ─── RequestClosureAsync ─────────────────────────────────────────────────

        /// <summary>
        /// US-16: Assigned agent requests closure → status becomes CLOSURE_REQUESTED
        /// with PENDING closure request.
        /// </summary>
        [Fact]
        public async Task RequestClosureAsync_ShouldSetClosureRequested_WhenAssignedAgentRequestsClosure()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act
            await _service.RequestClosureAsync(TicketId, AgentId, "AGENT");

            // Assert
            ticket.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestedById.Should().Be(AgentId);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);
            ticket.ClosureRequestedDate.Should().NotBeNull();
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-16: Technician assigned to ticket can also request closure.
        /// </summary>
        [Fact]
        public async Task RequestClosureAsync_ShouldSucceed_WhenAssignedTechnicianRequestsClosure()
        {
            // Arrange
            var ticket = MakeOpenTicket(TechId);
            ticket.Assignments.First().User = MakeUser(TechId, Role.TECHNICIAN);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act
            await _service.RequestClosureAsync(TicketId, TechId, "TECHNICIAN");

            // Assert
            ticket.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-16: Unassigned agent cannot request closure — throws UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task RequestClosureAsync_ShouldThrowUnauthorized_WhenAgentIsNotAssigned()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.RequestClosureAsync(TicketId, OtherUserId, "AGENT"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-16: CLIENT role cannot request closure — throws UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task RequestClosureAsync_ShouldThrowUnauthorized_WhenRoleIsClient()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.RequestClosureAsync(TicketId, ClientId, "CLIENT"));
        }

        /// <summary>
        /// US-16: Cannot request closure when ticket is already CLOSED.
        /// </summary>
        [Fact]
        public async Task RequestClosureAsync_ShouldThrowInvalidOperation_WhenTicketIsAlreadyClosed()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);
            ticket.Status = TicketStatus.CLOSED;
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RequestClosureAsync(TicketId, AgentId, "AGENT"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        // ─── AcceptClosureAsync ──────────────────────────────────────────────────

        /// <summary>
        /// US-16: Client accepts closure → ticket becomes CLOSED, request ACCEPTED.
        /// </summary>
        [Fact]
        public async Task AcceptClosureAsync_ShouldCloseTicket_WhenClientAcceptsClosure()
        {
            // Arrange
            var ticket = MakeClosureRequestedTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act
            await _service.AcceptClosureAsync(TicketId, ClientId);

            // Assert
            ticket.Status.Should().Be(TicketStatus.CLOSED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.ACCEPTED);
            ticket.ClosedDate.Should().NotBeNull();
            ticket.ClosedById.Should().Be(ClientId);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-16: Only the ticket creator can accept closure; another user throws UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task AcceptClosureAsync_ShouldThrowUnauthorized_WhenUserIsNotCreator()
        {
            // Arrange
            var ticket = MakeClosureRequestedTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.AcceptClosureAsync(TicketId, OtherUserId));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-16: Cannot accept closure when ticket has no pending closure request.
        /// </summary>
        [Fact]
        public async Task AcceptClosureAsync_ShouldThrowInvalidOperation_WhenNoClosureRequestPending()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);  // OPEN, no closure request
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AcceptClosureAsync(TicketId, ClientId));
        }

        // ─── RejectClosureAsync ──────────────────────────────────────────────────

        /// <summary>
        /// US-17: Client rejects closure → ticket returns to OPEN, request REJECTED.
        /// </summary>
        [Fact]
        public async Task RejectClosureAsync_ShouldReopenTicket_WhenClientRejectsClosure()
        {
            // Arrange
            var ticket = MakeClosureRequestedTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act
            await _service.RejectClosureAsync(TicketId, ClientId);

            // Assert
            ticket.Status.Should().Be(TicketStatus.OPEN);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.REJECTED);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-17: Only the ticket creator can reject closure — UnauthorizedAccessException for others.
        /// </summary>
        [Fact]
        public async Task RejectClosureAsync_ShouldThrowUnauthorized_WhenUserIsNotCreator()
        {
            // Arrange
            var ticket = MakeClosureRequestedTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.RejectClosureAsync(TicketId, OtherUserId));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-17: Cannot reject closure when no closure request is pending.
        /// </summary>
        [Fact]
        public async Task RejectClosureAsync_ShouldThrowInvalidOperation_WhenNoClosureRequestPending()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.RejectClosureAsync(TicketId, ClientId));
        }

        // ─── ForceCloseAsync (auto-close after 7 days) ──────────────────────────

        /// <summary>
        /// US-16: After 7 days without client response → agent can force-close.
        /// Uses DateTimeOffset simulation via CreatedDate/LastClientComment.
        /// </summary>
        [Fact]
        public async Task ForceCloseAsync_ShouldClosTicket_WhenSevenDaysPassedWithNoClientResponse()
        {
            // Arrange: ticket created 10 days ago, last client comment 9 days ago
            var ticket = MakeClosureRequestedTicket(AgentId);
            ticket.CreatedDate = DateTime.Now.AddDays(-10);
            ticket.Comments = new List<Comment>
            {
                new()
                {
                    CommentId = 1,
                    Content   = "Still waiting",
                    DateTime  = DateTime.Now.AddDays(-9), // > 7 days ago
                    AuthorId  = ClientId,
                    Author    = MakeUser(ClientId, Role.CLIENT),
                }
            };
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act
            await _service.ForceCloseAsync(TicketId, AgentId, "AGENT");

            // Assert
            ticket.Status.Should().Be(TicketStatus.CLOSED);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.EXPIRED);
            ticket.ClosedDate.Should().NotBeNull();
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-16: Force-close fails when client responded within last 7 days — no auto-close.
        /// </summary>
        [Fact]
        public async Task ForceCloseAsync_ShouldThrowInvalidOperation_WhenClientRespondedWithinSevenDays()
        {
            // Arrange: recent client comment (2 days ago — within 7-day window)
            var ticket = MakeClosureRequestedTicket(AgentId);
            ticket.Comments = new List<Comment>
            {
                new()
                {
                    CommentId = 1,
                    Content   = "Please keep it open",
                    DateTime  = DateTime.Now.AddDays(-2),
                    AuthorId  = ClientId,
                    Author    = MakeUser(ClientId, Role.CLIENT),
                }
            };
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ForceCloseAsync(TicketId, AgentId, "AGENT"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-16: Force-close requires CLOSURE_REQUESTED status; OPEN ticket throws InvalidOperationException.
        /// </summary>
        [Fact]
        public async Task ForceCloseAsync_ShouldThrowInvalidOperation_WhenTicketStatusIsNotClosureRequested()
        {
            // Arrange
            var ticket = MakeOpenTicket(AgentId);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ForceCloseAsync(TicketId, AgentId, "AGENT"));
        }

        /// <summary>
        /// US-16: Unassigned agent cannot force-close — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task ForceCloseAsync_ShouldThrowUnauthorized_WhenAgentIsNotAssigned()
        {
            // Arrange
            var ticket = MakeClosureRequestedTicket(AgentId);
            ticket.CreatedDate = DateTime.Now.AddDays(-10);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ForceCloseAsync(TicketId, OtherUserId, "AGENT"));
        }

        /// <summary>
        /// US-16: CLIENT role cannot force-close — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task ForceCloseAsync_ShouldThrowUnauthorized_WhenRoleIsClient()
        {
            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ForceCloseAsync(TicketId, ClientId, "CLIENT"));
        }
    }
}
