using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class TicketRepositoryTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private static Ticket MakeTicket(
            int id,
            int creatorId,
            string title = "Tiket",
            TicketStatus status = TicketStatus.OPEN,
            Priority priority = Priority.LOW,
            ProblemCategory category = ProblemCategory.INTERNET,
            DateTime? createdDate = null) => new()
        {
            TicketId = id,
            Title = title,
            Description = "Opis",
            CreatorId = creatorId,
            Status = status,
            Priority = priority,
            ProblemCategory = category,
            CreatedDate = createdDate ?? DateTime.UtcNow
        };

        [Fact]
        public async Task GetByCreatorIdAsync_ShouldReturnOnlyTicketsForGivenUser()
        {
            // Arrange
            using var context = CreateDbContext();

            context.Tickets.AddRange(
                new Ticket
                {
                    TicketId = 1,
                    Title = "Ticket 1",
                    CreatorId = 1,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.HIGH,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                },
                new Ticket
                {
                    TicketId = 2,
                    Title = "Ticket 2",
                    CreatorId = 2,
                    Status = TicketStatus.OPEN,
                    Priority = Priority.LOW,
                    ProblemCategory = ProblemCategory.TV,
                    CreatedDate = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();

            var repository = new TicketRepository(context);

            // Act
            var result = await repository.GetByCreatorIdAsync(1);

            // Assert
            result.Should().HaveCount(1);
            result.First().CreatorId.Should().Be(1);
            result.First().Title.Should().Be("Ticket 1");
        }

        // ─── GetByCreatorIdAsync prazna lista (US-11, US-13) ─────────────────────

        // US-11/US-13: korisnik bez tiketa dobija praznu listu
        [Fact]
        public async Task GetByCreatorIdAsync_ShouldReturnEmptyList_WhenUserHasNoTickets()
        {
            using var context = CreateDbContext();
            var repository = new TicketRepository(context);

            var result = await repository.GetByCreatorIdAsync(99);

            Assert.Empty(result);
        }

        // ─── GetByCreatorIdAsync sortiranje (US-11) ───────────────────────────────

        // US-11: tiketi su sortirani od najnovijeg prema najstarijem (za pregled u interfejsu)
        [Fact]
        public async Task GetByCreatorIdAsync_ShouldReturnTicketsOrderedByCreatedDateDescending()
        {
            using var context = CreateDbContext();
            var older = MakeTicket(id: 1, creatorId: 1, title: "Stariji", createdDate: DateTime.UtcNow.AddDays(-5));
            var newer = MakeTicket(id: 2, creatorId: 1, title: "Noviji",  createdDate: DateTime.UtcNow);
            context.Tickets.AddRange(older, newer);
            await context.SaveChangesAsync();

            var result = (await new TicketRepository(context).GetByCreatorIdAsync(1)).ToList();

            Assert.Equal("Noviji",  result[0].Title);
            Assert.Equal("Stariji", result[1].Title);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddTicketToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repository = new TicketRepository(context);

            var ticket = new Ticket
            {
                Title = "New ticket",
                Description = "Description",
                CreatorId = 1,
                Status = TicketStatus.OPEN,
                Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            };

            // Act
            var result = await repository.CreateAsync(ticket);

            // Assert
            result.TicketId.Should().BeGreaterThan(0);
            context.Tickets.Should().HaveCount(1);
            context.Tickets.First().Title.Should().Be("New ticket");
        }

        // ─── CreateAsync sva polja (US-8, US-9, US-10) ───────────────────────────

        // US-8/US-9/US-10: sva polja tiketa se ispravno čuvaju u bazi
        [Fact]
        public async Task CreateAsync_ShouldPersistAllTicketFields()
        {
            using var context = CreateDbContext();
            var repository = new TicketRepository(context);
            var now = DateTime.UtcNow;

            var ticket = new Ticket
            {
                Title = "TV problem",
                Description = "Signal ne radi",
                CreatorId = 3,
                Status = TicketStatus.OPEN,
                Priority = Priority.HIGH,
                ProblemCategory = ProblemCategory.TV,
                CreatedDate = now
            };

            var result = await repository.CreateAsync(ticket);

            Assert.Equal("TV problem",           result.Title);
            Assert.Equal("Signal ne radi",       result.Description);
            Assert.Equal(3,                      result.CreatorId);
            Assert.Equal(TicketStatus.OPEN,      result.Status);
            Assert.Equal(Priority.HIGH,          result.Priority);
            Assert.Equal(ProblemCategory.TV,     result.ProblemCategory);
        }
    }
}