using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    // PB-62 / US-105: Unit testovi samodjelovanja tiketa (agent „Preuzmi tiket")
    public class SelfAssignServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepoMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<ICommentService> _commentServiceMock = new();
        private readonly TicketService _service;

        public SelfAssignServiceTests()
        {
            _service = new TicketService(
                _ticketRepoMock.Object,
                _teamRepoMock.Object,
                _userRepoMock.Object,
                _notificationServiceMock.Object,
                _commentServiceMock.Object);
        }

        private static Ticket MakeTicket(int id = 1, TicketStatus status = TicketStatus.OPEN, int? teamId = 1)
            => new()
            {
                TicketId = id,
                Title = "Test tiket",
                Description = "Opis",
                CreatorId = 99,
                Status = status,
                Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                TeamId = teamId,
                Assignments = new List<TicketUser>()
            };

        private static User MakeAgent(int id = 10, int? teamId = 1) => new()
        {
            UserId = id,
            FirstName = "Agent",
            LastName = id.ToString(),
            Email = $"a{id}@test.ba",
            Username = $"a{id}",
            PasswordHash = "h",
            Role = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            TeamId = teamId,
        };

        // ─── Pozitivan slučaj ─────────────────────────────────────────────────────────

        [Fact]
        public async Task SelfAssignTicketAsync_ShouldAssignTicket_ToCallingAgent()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(MakeAgent());

            var result = await _service.SelfAssignTicketAsync(1, 10);

            result.UserId.Should().Be(10);
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.Is<TicketUser>(
                a => a.UserId == 10
                  && a.TicketId == 1
                  && a.AssignmentType == AssignmentType.MANUAL
                  && a.TeamId == 1)), Times.Once);
        }

        // PB-62: nakon uspjeha mora se obavijestiti klijent i dodati zapis u istoriju tiketa
        [Fact]
        public async Task SelfAssignTicketAsync_ShouldNotifyClientAndAddSystemComment_OnSuccess()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(MakeAgent());

            await _service.SelfAssignTicketAsync(1, 10);

            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                99,
                It.IsAny<string>(),
                It.IsAny<string>(),
                NotificationType.TICKET_ASSIGNED,
                1), Times.Once);
            _commentServiceMock.Verify(c => c.AddSystemCommentAsync(
                1,
                It.Is<string>(s => s.Contains("preuzeo"))), Times.Once);
        }

        // ─── Negativni slučajevi ──────────────────────────────────────────────────────

        [Fact]
        public async Task SelfAssignTicketAsync_ShouldThrowKeyNotFound_WhenTicketMissing()
        {
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(7)).ReturnsAsync((Ticket?)null);

            var act = () => _service.SelfAssignTicketAsync(7, 10);

            await act.Should().ThrowAsync<KeyNotFoundException>();
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Never);
        }

        [Fact]
        public async Task SelfAssignTicketAsync_ShouldThrow_WhenTicketIsClosed()
        {
            var ticket = MakeTicket(status: TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            var act = () => _service.SelfAssignTicketAsync(1, 10);

            await act.Should().ThrowAsync<InvalidOperationException>();
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Never);
        }

        // PB-62: agent ne smije preuzeti tiket koji je već dodijeljen drugom agentu
        [Fact]
        public async Task SelfAssignTicketAsync_ShouldThrow_WhenTicketAlreadyAssigned()
        {
            var ticket = MakeTicket();
            ticket.Assignments.Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = 1,
                UserId = 55,
                TeamId = 1,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.AUTOMATIC,
                User = new User { UserId = 55, FirstName = "Drugi", LastName = "Agent", Role = Role.AGENT }
            });
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            var act = () => _service.SelfAssignTicketAsync(1, 10);

            (await act.Should().ThrowAsync<InvalidOperationException>())
                .WithMessage("*već dodijeljen*");
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Never);
        }

        // PB-62: klijent/tehničar/administrator ne mogu raditi self-assign — provjera role
        [Fact]
        public async Task SelfAssignTicketAsync_ShouldThrow_WhenCallerIsNotAgent()
        {
            var ticket = MakeTicket();
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new User
            {
                UserId = 10,
                FirstName = "Tech",
                LastName = "User",
                Email = "t@t.ba",
                Username = "t",
                PasswordHash = "h",
                Role = Role.TECHNICIAN,
                AccountStatus = AccountStatus.ACTIVE,
            });

            var act = () => _service.SelfAssignTicketAsync(1, 10);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task SelfAssignTicketAsync_ShouldThrow_WhenAgentHasNoTeamAndTicketHasNoTeam()
        {
            var ticket = MakeTicket(teamId: null);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _userRepoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(MakeAgent(teamId: null));

            var act = () => _service.SelfAssignTicketAsync(1, 10);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
