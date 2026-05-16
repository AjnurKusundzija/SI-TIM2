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
    public class AllTicketsServiceTests
    {
        private readonly Mock<ITicketRepository> _repoMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly TicketService _service;

        public AllTicketsServiceTests()
        {
            _service = new TicketService(_repoMock.Object, _teamRepoMock.Object, _userRepoMock.Object, _notificationServiceMock.Object, new Mock<ICommentService>().Object);
        }

        private static Ticket MakeTicket(int id = 1, int creatorId = 5) => new()
        {
            TicketId = id,
            Title = $"Tiket {id}",
            Description = "Opis",
            CreatorId = creatorId,
            Status = TicketStatus.OPEN,
            Priority = Priority.HIGH,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = DateTime.UtcNow
        };

        // PB-32: agent bez assignedOnly dohvata sve tikete
        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnAllTickets_WhenAgentCallsWithoutAssignedOnly()
        {
            var all = new List<Ticket> { MakeTicket(1), MakeTicket(2), MakeTicket(3) };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(all);

            var result = await _service.GetAllTicketsAsync(99, "AGENT", assignedOnly: false);

            result.Should().HaveCount(3);
            _repoMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        // PB-32: agent sa assignedOnly=true dohvata samo dodijeljene tikete
        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnAssignedOnly_WhenAgentSetsAssignedOnlyTrue()
        {
            var assigned = new List<Ticket> { MakeTicket(1) };
            _repoMock.Setup(r => r.GetByAssigneeIdAsync(99)).ReturnsAsync(assigned);

            var result = await _service.GetAllTicketsAsync(99, "AGENT", assignedOnly: true);

            result.Should().HaveCount(1);
            _repoMock.Verify(r => r.GetByAssigneeIdAsync(99), Times.Once);
        }

        // PB-32: tehničar uvijek dohvata samo dodijeljene tikete
        [Fact]
        public async Task GetAllTicketsAsync_ShouldReturnAssignedOnly_WhenTechnicianCalls()
        {
            var assigned = new List<Ticket> { MakeTicket(2, 10) };
            _repoMock.Setup(r => r.GetByAssigneeIdAsync(55)).ReturnsAsync(assigned);

            var result = await _service.GetAllTicketsAsync(55, "TECHNICIAN");

            result.Should().HaveCount(1);
            _repoMock.Verify(r => r.GetByAssigneeIdAsync(55), Times.Once);
        }

        // sigurnosno: klijent ne može pozvati GetAllTickets — baca UnauthorizedAccessException
        [Fact]
        public async Task GetAllTicketsAsync_ShouldThrowUnauthorized_WhenClientCalls()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetAllTicketsAsync(1, "CLIENT"));
        }
    }
}
