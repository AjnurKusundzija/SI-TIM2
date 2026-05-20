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
    public class TicketDetailServiceTests
    {
        private readonly Mock<ITicketRepository> _repoMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly TicketService _service;

        public TicketDetailServiceTests()
        {
            _service = new TicketService(_repoMock.Object, _teamRepoMock.Object, _userRepoMock.Object, _notificationServiceMock.Object, new Mock<ICommentService>().Object);
        }

        private static User MakeUser(int id = 1, string first = "Amir", string last = "Hadzic", Role role = Role.CLIENT) =>
            new() { UserId = id, FirstName = first, LastName = last, Role = role };

        private static Ticket MakeTicket(int id = 1, int creatorId = 5,
            TicketStatus status = TicketStatus.OPEN,
            Priority priority = Priority.HIGH,
            ProblemCategory category = ProblemCategory.INTERNET,
            List<TicketUser>? assignments = null) => new()
        {
            TicketId = id,
            Title = "Internet ne radi",
            Description = "Nema veze od jutros.",
            CreatedDate = new DateTime(2026, 5, 1, 10, 0, 0),
            Status = status,
            Priority = priority,
            ProblemCategory = category,
            CreatorId = creatorId,
            Creator = MakeUser(creatorId, "Merjem", "Omerović"),
            Assignments = assignments ?? new List<TicketUser>(),
        };

        // US-14: klijent može vidjeti vlastiti tiket
        [Fact]
        public async Task GetTicketByIdAsync_ShouldReturnDto_WhenClientIsOwner()
        {
            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(MakeTicket(creatorId: 5));

            var dto = await _service.GetTicketByIdAsync(1, 5, "CLIENT");

            dto.Should().NotBeNull();
            dto.TicketId.Should().Be(1);
        }

        // US-14: tiket koji ne postoji baca KeyNotFoundException
        [Fact]
        public async Task GetTicketByIdAsync_ShouldThrowKeyNotFound_WhenTicketMissing()
        {
            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((Ticket?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetTicketByIdAsync(99, 1, "CLIENT"));
        }

        // US-14: klijent ne može vidjeti tuđi tiket (sigurnosno odvajanje podataka)
        [Fact]
        public async Task GetTicketByIdAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket()
        {
            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(MakeTicket(creatorId: 5));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetTicketByIdAsync(1, userId: 99, "CLIENT"));
        }

        // US-30: agent može vidjeti svaki tiket bez obzira na vlasništvo
        [Fact]
        public async Task GetTicketByIdAsync_ShouldReturnDto_WhenAgentAccessesAnyTicket()
        {
            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(MakeTicket(creatorId: 5));

            var dto = await _service.GetTicketByIdAsync(1, userId: 99, "AGENT");

            dto.Should().NotBeNull();
        }

        // US-14: sva polja tiketa se ispravno mapiraju u DTO
        [Fact]
        public async Task GetTicketByIdAsync_ShouldMapAllFieldsToDto()
        {
            var agentUser = MakeUser(10, "Selma", "Mujić", Role.AGENT);
            var technicianUser = MakeUser(11, "Adnan", "Hasić", Role.TECHNICIAN);
            var baseDate = DateTime.UtcNow.AddMinutes(-10);
            var assignment = new TicketUser
            {
                AssignmentId = 1,
                TicketId = 1,
                UserId = agentUser.UserId,
                User = agentUser,
                AssignmentDate = baseDate,
                TeamId = 0,
                AssignmentType = AssignmentType.AUTOMATIC
            };
            var technicianAssignment = new TicketUser
            {
                AssignmentId = 2,
                TicketId = 1,
                UserId = technicianUser.UserId,
                User = technicianUser,
                AssignmentDate = baseDate.AddMinutes(1),
                TeamId = 0,
                AssignmentType = AssignmentType.FORWARDED_TO_TECHNICIAN
            };
            var ticket = MakeTicket(id: 1, creatorId: 5, assignments: new List<TicketUser> { assignment, technicianAssignment });

            _repoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            var dto = await _service.GetTicketByIdAsync(1, 5, "CLIENT");

            dto.Title.Should().Be("Internet ne radi");
            dto.Description.Should().Be("Nema veze od jutros.");
            dto.Status.Should().Be(TicketStatus.OPEN.ToString());
            dto.Priority.Should().Be(Priority.HIGH.ToString());
            dto.ProblemCategory.Should().Be(ProblemCategory.INTERNET.ToString());
            dto.ClientName.Should().Be("Merjem Omerović");
            dto.AssignedAgentName.Should().Be("Selma Mujić");
            dto.AssignedAgentId.Should().Be(10);
            dto.AssignedTechnicianName.Should().Be("Adnan Hasić");
            dto.AssignedTechnicianId.Should().Be(11);
        }
    }
}
