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
    /// US-21 / US-22 – Internal priority management unit tests.
    /// Tests UpdateInternalPriorityAsync in TicketService.
    /// </summary>
    public class TicketPriorityServiceTests
    {
        private readonly Mock<ITicketRepository>    _ticketRepoMock     = new();
        private readonly Mock<ITeamRepository>      _teamRepoMock       = new();
        private readonly Mock<IUserRepository>      _userRepoMock       = new();
        private readonly Mock<INotificationService> _notificationMock   = new();
        private readonly Mock<ICommentService>      _commentServiceMock = new();
        private readonly TicketService              _service;

        private const int AgentId     = 1;
        private const int AdminId     = 2;
        private const int ClientId    = 3;
        private const int TicketId    = 10;

        public TicketPriorityServiceTests()
        {
            _service = new TicketService(
                _ticketRepoMock.Object,
                _teamRepoMock.Object,
                _userRepoMock.Object,
                _notificationMock.Object,
                _commentServiceMock.Object);
        }

        private static Ticket MakeTicket(int id = TicketId) => new()
        {
            TicketId        = id,
            Title           = "Network outage",
            Description     = "No internet",
            CreatorId       = ClientId,
            Status          = TicketStatus.OPEN,
            Priority        = Priority.LOW,
            InternalPriority = null,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = DateTime.UtcNow,
        };

        // ─── Agent can change priority ────────────────────────────────────────

        /// <summary>
        /// US-21: Agent can set internal priority to LOW.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldSetLow_WhenAgentChangesToLow()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdAsync(TicketId)).ReturnsAsync(ticket);

            await _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.LOW, AgentId, "AGENT");

            ticket.InternalPriority.Should().Be(InternalPriority.LOW);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-21: Agent can set internal priority to MEDIUM.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldSetMedium_WhenAgentChangesToMedium()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdAsync(TicketId)).ReturnsAsync(ticket);

            await _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.MEDIUM, AgentId, "AGENT");

            ticket.InternalPriority.Should().Be(InternalPriority.MEDIUM);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-21: Agent can set internal priority to HIGH.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldSetHigh_WhenAgentChangesToHigh()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdAsync(TicketId)).ReturnsAsync(ticket);

            await _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.HIGH, AgentId, "AGENT");

            ticket.InternalPriority.Should().Be(InternalPriority.HIGH);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        /// <summary>
        /// US-21: Admin can also change internal priority.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldSucceed_WhenAdminChangesPriority()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdAsync(TicketId)).ReturnsAsync(ticket);

            await _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.CRITICAL, AdminId, "ADMINISTRATOR");

            ticket.InternalPriority.Should().Be(InternalPriority.CRITICAL);
            _ticketRepoMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
        }

        // ─── Client cannot change priority ────────────────────────────────────

        /// <summary>
        /// US-21: CLIENT role cannot change internal priority — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldThrowUnauthorized_WhenRoleIsClient()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.HIGH, ClientId, "CLIENT"));

            _ticketRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        /// <summary>
        /// US-21: TECHNICIAN role cannot change internal priority — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldThrowUnauthorized_WhenRoleIsTechnician()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateInternalPriorityAsync(TicketId, InternalPriority.HIGH, 5, "TECHNICIAN"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        // ─── Non-existent ticket ──────────────────────────────────────────────

        /// <summary>
        /// US-21: Non-existent ticket throws KeyNotFoundException.
        /// </summary>
        [Fact]
        public async Task UpdateInternalPriorityAsync_ShouldThrowKeyNotFound_WhenTicketDoesNotExist()
        {
            _ticketRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Ticket?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateInternalPriorityAsync(999, InternalPriority.HIGH, AgentId, "AGENT"));

            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }
    }
}
