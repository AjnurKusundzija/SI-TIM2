using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Communication
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _commentRepoMock = new();
        private readonly Mock<ITicketRepository> _ticketRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _service = new CommentService(_commentRepoMock.Object, _ticketRepoMock.Object, _notificationServiceMock.Object);
        }

        private static User MakeUser(int id = 1, Role role = Role.CLIENT) => new()
        {
            UserId = id, FirstName = "Amira", LastName = "Begić", Role = role
        };

        private static Ticket MakeTicket(int id = 1, int creatorId = 5, List<TicketUser>? assignments = null) => new()
        {
            TicketId = id,
            Title = "Internet problem",
            CreatorId = creatorId,
            Status = TicketStatus.OPEN,
            Creator = MakeUser(creatorId),
            Assignments = assignments ?? new List<TicketUser>()
        };

        private static Comment MakeComment(int id, int ticketId, int authorId, string content = "Test poruka") => new()
        {
            CommentId = id,
            TicketId = ticketId,
            AuthorId = authorId,
            Content = content,
            DateTime = DateTime.UtcNow,
            Author = MakeUser(authorId)
        };

        // US-20: klijent može poslati poruku na vlastiti tiket
        [Fact]
        public async Task AddCommentAsync_ShouldSucceed_WhenClientIsOwnerAndContentValid()
        {
            var ticket = MakeTicket(creatorId: 5);
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);

            var newComment = MakeComment(10, 1, 5, "Hvala na brzom odgovoru!");
            _commentRepoMock.Setup(r => r.CreateAsync(It.IsAny<Comment>())).ReturnsAsync(newComment);
            _commentRepoMock.Setup(r => r.GetByTicketIdAsync(1))
                .ReturnsAsync(new List<Comment> { newComment });

            var result = await _service.AddCommentAsync(1, 5, "CLIENT", "Hvala na brzom odgovoru!");

            result.Should().NotBeNull();
            result.AuthorId.Should().Be(5);
        }

        // US-20: poruka duža od 1000 znakova se odbija (NFR validacija)
        [Fact]
        public async Task AddCommentAsync_ShouldThrow_WhenContentExceeds1000Characters()
        {
            var longContent = new string('a', 1001);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.AddCommentAsync(1, 5, "CLIENT", longContent));
        }

        // US-20: tiket koji ne postoji baca KeyNotFoundException
        [Fact]
        public async Task AddCommentAsync_ShouldThrowKeyNotFound_WhenTicketDoesNotExist()
        {
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(99)).ReturnsAsync((Ticket?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddCommentAsync(99, 5, "CLIENT", "Poruka"));
        }

        // sigurnosno: klijent ne može slati poruke na tuđi tiket
        [Fact]
        public async Task AddCommentAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherTicket()
        {
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(MakeTicket(creatorId: 5));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.AddCommentAsync(1, userId: 99, "CLIENT", "Poruka"));
        }

        // US-15: klijent može dohvatiti historiju poruka za vlastiti tiket
        [Fact]
        public async Task GetCommentsForTicketAsync_ShouldReturnComments_WhenClientIsOwner()
        {
            var ticket = MakeTicket(creatorId: 5);
            var comments = new List<Comment>
            {
                MakeComment(1, 1, 5, "Prva poruka"),
                MakeComment(2, 1, 10, "Odgovor agenta"),
            };
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(ticket);
            _commentRepoMock.Setup(r => r.GetByTicketIdAsync(1)).ReturnsAsync(comments);

            var result = await _service.GetCommentsForTicketAsync(1, 5, "CLIENT");

            result.Should().HaveCount(2);
        }

        // sigurnosno: klijent ne može vidjeti poruke tuđeg tiketa
        [Fact]
        public async Task GetCommentsForTicketAsync_ShouldThrowUnauthorized_WhenClientNotOwner()
        {
            _ticketRepoMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(MakeTicket(creatorId: 5));

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetCommentsForTicketAsync(1, requestingUserId: 99, "CLIENT"));
        }
    }
}
