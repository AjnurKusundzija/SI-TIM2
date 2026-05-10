using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketService = new TicketService(_ticketRepositoryMock.Object, _teamRepoMock.Object, _userRepoMock.Object);
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private static Ticket MakeTicket(
            int id = 1,
            int creatorId = 1,
            TicketStatus status = TicketStatus.OPEN,
            Priority priority = Priority.LOW,
            ProblemCategory category = ProblemCategory.INTERNET) => new()
        {
            TicketId = id,
            Title = "Test tiket",
            Description = "Opis",
            CreatorId = creatorId,
            Status = status,
            Priority = priority,
            ProblemCategory = category,
            CreatedDate = DateTime.UtcNow
        };

        private static CreateTicketDto MakeCreateDto(
            string subject = "Subject",
            string description = "Description",
            Priority priority = Priority.LOW,
            ProblemCategory category = ProblemCategory.INTERNET) => new()
        {
            Subject = subject,
            Description = description,
            Priority = priority,
            Type = category
        };

        [Fact]
        public async Task GetMyTicketsAsync_ShouldReturnOnlyMappedTickets()
        {
            // Arrange
            var userId = 1;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    TicketId = 10,
                    Title = "Internet problem",
                    Description = "Internet ne radi",
                    CreatorId = userId,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.HIGH,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate = new DateTime(2026, 1, 1)
                }
            };

            _ticketRepositoryMock
                .Setup(r => r.GetByCreatorIdAsync(userId))
                .ReturnsAsync(tickets);

            // Act
            var result = await _ticketService.GetMyTicketsAsync(userId);

            // Assert
            result.Should().HaveCount(1);

            var ticket = result.First();
            ticket.TicketId.Should().Be(10);
            ticket.Title.Should().Be("Internet problem");
            ticket.Status.Should().Be(TicketStatus.OPEN.ToString());
            ticket.Priority.Should().Be(Priority.HIGH.ToString());
            ticket.ProblemCategory.Should().Be(ProblemCategory.INTERNET.ToString());

            _ticketRepositoryMock.Verify(r => r.GetByCreatorIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateTicketAsync_ShouldCreateTicketAndReturnDto()
        {
            // Arrange
            var userId = 5;

            var createDto = new CreateTicketDto
            {
                Subject = "Problem sa TV",
                Description = "TV signal ne radi",
                Priority = Priority.MEDIUM,
                Type = ProblemCategory.TV
            };

            _ticketRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket ticket) =>
                {
                    ticket.TicketId = 100;
                    return ticket;
                });

            // Act
            var result = await _ticketService.CreateTicketAsync(createDto, userId);

            // Assert
            result.Should().NotBeNull();
            result.TicketId.Should().Be(100);
            result.Title.Should().Be("Problem sa TV");
            result.Description.Should().Be("TV signal ne radi");
            result.CreatorId.Should().Be(userId);
            result.Status.Should().Be(TicketStatus.OPEN.ToString());
            result.Priority.Should().Be(Priority.MEDIUM.ToString());
            result.ProblemCategory.Should().Be(ProblemCategory.TV.ToString());

            _ticketRepositoryMock.Verify(r => r.CreateAsync(It.Is<Ticket>(t =>
                t.Title == createDto.Subject &&
                t.Description == createDto.Description &&
                t.CreatorId == userId &&
                t.Status == TicketStatus.OPEN &&
                t.Priority == createDto.Priority &&
                t.ProblemCategory == createDto.Type
            )), Times.Once);
        }

        // ─── GetMyTicketsAsync prazna lista (US-11, US-13) ───────────────────────

        // US-11/US-13: korisnik bez tiketa dobija praznu listu (poruka kada nema tiketa)
        [Fact]
        public async Task GetMyTicketsAsync_ShouldReturnEmptyList_WhenUserHasNoTickets()
        {
            _ticketRepositoryMock.Setup(r => r.GetByCreatorIdAsync(99)).ReturnsAsync(new List<Ticket>());

            var result = await _ticketService.GetMyTicketsAsync(99);

            Assert.Empty(result);
        }

        // ─── GetMyTicketsAsync mapiranje statusa (US-12) ──────────────────────────

        // US-12: oba statusa se ispravno mapiraju u string za prikaz u interfejsu
        [Theory]
        [InlineData(TicketStatus.OPEN, "OPEN")]
        [InlineData(TicketStatus.CLOSED, "CLOSED")]
        public async Task GetMyTicketsAsync_ShouldMapStatusToString(TicketStatus status, string expected)
        {
            _ticketRepositoryMock
                .Setup(r => r.GetByCreatorIdAsync(1))
                .ReturnsAsync(new List<Ticket> { MakeTicket(status: status) });

            var result = await _ticketService.GetMyTicketsAsync(1);

            Assert.Equal(expected, result.Single().Status);
        }

        // ─── CreateTicketAsync poslovne vrijednosti (US-8, US-9) ─────────────────

        // US-8: novi tiket uvijek dobija status OPEN pri kreiranju
        [Fact]
        public async Task CreateTicketAsync_ShouldAlwaysSetStatusToOpen()
        {
            _ticketRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => t);

            var result = await _ticketService.CreateTicketAsync(MakeCreateDto(), 1);

            Assert.Equal(TicketStatus.OPEN.ToString(), result.Status);
        }

        // US-9: svi prioriteti se ispravno prenose i mapiraju u string
        [Theory]
        [InlineData(Priority.LOW)]
        [InlineData(Priority.MEDIUM)]
        [InlineData(Priority.HIGH)]
        public async Task CreateTicketAsync_ShouldPreserveAllPriorities(Priority priority)
        {
            _ticketRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => t);

            var result = await _ticketService.CreateTicketAsync(MakeCreateDto(priority: priority), 1);

            Assert.Equal(priority.ToString(), result.Priority);
        }

        // US-9: svi tipovi problema se ispravno prenose i mapiraju u string
        [Theory]
        [InlineData(ProblemCategory.INTERNET)]
        [InlineData(ProblemCategory.TV)]
        [InlineData(ProblemCategory.MOBILE_NETWORK)]
        [InlineData(ProblemCategory.BILLING)]
        [InlineData(ProblemCategory.TECHNICAL_SUPPORT)]
        public async Task CreateTicketAsync_ShouldPreserveAllProblemCategories(ProblemCategory category)
        {
            _ticketRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => t);

            var result = await _ticketService.CreateTicketAsync(MakeCreateDto(category: category), 1);

            Assert.Equal(category.ToString(), result.ProblemCategory);
        }
    }
}