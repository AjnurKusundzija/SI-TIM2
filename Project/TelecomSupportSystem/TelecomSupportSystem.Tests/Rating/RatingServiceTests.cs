using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Ratings;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Rating
{
    public class RatingServiceTests
    {
        private readonly Mock<IRatingRepository> _ratingRepoMock = new();
        private readonly Mock<ITicketRepository> _ticketRepoMock = new();
        private readonly RatingService _service;

        public RatingServiceTests()
        {
            _service = new RatingService(_ratingRepoMock.Object, _ticketRepoMock.Object);
        }

        private static User MakeUser(int id, Role role = Role.CLIENT) => new()
        {
            UserId = id, FirstName = "Amira", LastName = "Begić", Role = role,
            Email = $"user{id}@test.ba", Username = $"user{id}", PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
        };

        private static Ticket MakeTicket(int id, int creatorId, TicketStatus status) => new()
        {
            TicketId = id,
            Title = "Internet problem",
            CreatorId = creatorId,
            Status = status,
            Creator = MakeUser(creatorId),
            Assignments = new List<TicketUser>(),
        };

        // US-61: sretan slučaj — klijent ocjenjuje vlastiti zatvoren tiket
        [Fact]
        public async Task CreateRatingAsync_ShouldSucceed_WhenClientRatesOwnClosedTicket()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync((DAL.Entities.Rating?)null);
            _ratingRepoMock.Setup(r => r.CreateAsync(It.IsAny<DAL.Entities.Rating>()))
                .ReturnsAsync((DAL.Entities.Rating r) => r);

            var dto = new CreateRatingDto { RatingValue = 4, RatingComment = "Odlično!" };
            var result = await _service.CreateRatingAsync(1, userId: 5, "CLIENT", dto);

            result.Should().NotBeNull();
            result.RatingValue.Should().Be(4);
            result.RatingComment.Should().Be("Odlično!");
        }

        // US-61: komentar je null → snima prazni string
        [Fact]
        public async Task CreateRatingAsync_ShouldSaveEmptyString_WhenCommentIsNull()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync((DAL.Entities.Rating?)null);
            _ratingRepoMock.Setup(r => r.CreateAsync(It.IsAny<DAL.Entities.Rating>()))
                .ReturnsAsync((DAL.Entities.Rating r) => r);

            var dto = new CreateRatingDto { RatingValue = 3, RatingComment = null };
            var result = await _service.CreateRatingAsync(1, userId: 5, "CLIENT", dto);

            result.RatingComment.Should().Be(string.Empty);
        }

        // AC: tiket nije CLOSED → InvalidOperationException
        [Fact]
        public async Task CreateRatingAsync_ShouldThrow_WhenTicketIsNotClosed()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.OPEN);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync((DAL.Entities.Rating?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRatingAsync(1, userId: 5, "CLIENT", new CreateRatingDto { RatingValue = 5 }));
        }

        // AC: tiket već ocijenjen → InvalidOperationException
        [Fact]
        public async Task CreateRatingAsync_ShouldThrow_WhenTicketAlreadyRated()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            var existingRating = new DAL.Entities.Rating { RatingId = 1, TicketId = 1, UserId = 5, RatingValue = 3 };
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync(existingRating);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateRatingAsync(1, userId: 5, "CLIENT", new CreateRatingDto { RatingValue = 5 }));
        }

        // AC: pogrešna rola (AGENT) → UnauthorizedAccessException
        [Fact]
        public async Task CreateRatingAsync_ShouldThrow_WhenRoleIsNotClient()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CreateRatingAsync(1, userId: 10, "AGENT", new CreateRatingDto { RatingValue = 4 }));
        }

        // AC: klijent nije kreator tiketa → UnauthorizedAccessException
        [Fact]
        public async Task CreateRatingAsync_ShouldThrow_WhenClientIsNotOwner()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CreateRatingAsync(1, userId: 99, "CLIENT", new CreateRatingDto { RatingValue = 4 }));
        }

        // GetRating: agent vidi ocjenu
        [Fact]
        public async Task GetRatingForTicketAsync_ShouldReturnRating_WhenAgentRequests()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            var existingRating = new DAL.Entities.Rating
            {
                RatingId = 1, TicketId = 1, UserId = 5, RatingValue = 5,
                RatingComment = "Super", RatingDate = DateTime.UtcNow
            };
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync(existingRating);

            var result = await _service.GetRatingForTicketAsync(1, requestingUserId: 10, "AGENT");

            result.Should().NotBeNull();
            result!.RatingValue.Should().Be(5);
        }

        // GetRating: TECHNICIAN ne smije vidjeti ocjenu → UnauthorizedAccessException
        [Fact]
        public async Task GetRatingForTicketAsync_ShouldThrowUnauthorized_WhenTechnicianRequests()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetRatingForTicketAsync(1, requestingUserId: 10, "TECHNICIAN"));
        }

        // GetRating: nema ocjene → vraća null
        [Fact]
        public async Task GetRatingForTicketAsync_ShouldReturnNull_WhenNoRatingExists()
        {
            var ticket = MakeTicket(1, creatorId: 5, TicketStatus.CLOSED);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _ratingRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync((DAL.Entities.Rating?)null);

            var result = await _service.GetRatingForTicketAsync(1, requestingUserId: 5, "CLIENT");

            result.Should().BeNull();
        }
    }
}
