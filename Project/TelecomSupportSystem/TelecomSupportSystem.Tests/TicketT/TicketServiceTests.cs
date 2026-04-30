using FluentAssertions;
using Moq;
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
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _ticketService = new TicketService(_ticketRepositoryMock.Object);
        }

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
    }
}