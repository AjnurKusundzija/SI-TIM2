using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class AllTicketsRepositoryTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static Ticket MakeTicket(int id, int creatorId, DateTime? date = null) => new()
        {
            TicketId = id,
            Title = $"Tiket {id}",
            Description = "Opis",
            CreatorId = creatorId,
            Status = TicketStatus.OPEN,
            Priority = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = date ?? DateTime.UtcNow
        };

        // PB-32: GetAllAsync vraća sve tikete bez obzira na kreatora
        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTickets()
        {
            using var context = CreateDbContext();
            context.Tickets.AddRange(MakeTicket(1, 10), MakeTicket(2, 20), MakeTicket(3, 30));
            await context.SaveChangesAsync();

            var result = await new TicketRepository(context).GetAllAsync();

            result.Should().HaveCount(3);
        }

        // PB-32: GetAllAsync vraća praznu listu kada nema tiketa
        [Fact]
        public async Task GetAllAsync_ShouldReturnEmpty_WhenNoTicketsExist()
        {
            using var context = CreateDbContext();

            var result = await new TicketRepository(context).GetAllAsync();

            result.Should().BeEmpty();
        }

        // PB-32: GetAllAsync vraća tikete sortirane od najnovijeg ka najstarijem
        [Fact]
        public async Task GetAllAsync_ShouldReturnTicketsOrderedByCreatedDateDescending()
        {
            using var context = CreateDbContext();
            context.Tickets.AddRange(
                MakeTicket(1, 1, DateTime.UtcNow.AddDays(-5)),
                MakeTicket(2, 2, DateTime.UtcNow),
                MakeTicket(3, 3, DateTime.UtcNow.AddDays(-2)));
            await context.SaveChangesAsync();

            var result = (await new TicketRepository(context).GetAllAsync()).ToList();

            result[0].TicketId.Should().Be(2);
            result[1].TicketId.Should().Be(3);
            result[2].TicketId.Should().Be(1);
        }

        // PB-32: GetByAssigneeIdAsync vraća samo tikete na kojima je korisnik dodijeljen
        [Fact]
        public async Task GetByAssigneeIdAsync_ShouldReturnOnlyAssignedTickets()
        {
            using var context = CreateDbContext();
            context.Tickets.AddRange(MakeTicket(1, 10), MakeTicket(2, 20));
            await context.SaveChangesAsync();

            // Dodijeliti agenta 99 samo na tiket 1
            context.Set<TicketUser>().Add(new TicketUser
            {
                AssignmentId = 1,
                TicketId = 1,
                UserId = 99,
                TeamId = 0,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.MANUAL
            });
            await context.SaveChangesAsync();

            var result = await new TicketRepository(context).GetByAssigneeIdAsync(99);

            result.Should().HaveCount(1);
            result.First().TicketId.Should().Be(1);
        }
    }
}
