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
    }
}