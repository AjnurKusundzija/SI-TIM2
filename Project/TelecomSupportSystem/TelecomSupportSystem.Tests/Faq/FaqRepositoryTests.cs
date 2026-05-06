using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Faq
{
    public class FaqRepositoryTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnOnlyActiveFaqsOrderedBySortOrder()
        {
            using var context = CreateDbContext();
            context.Faqs.AddRange(
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 1,
                    Question = "Second",
                    Answer = "Answer",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 2,
                    Question = "Inactive",
                    Answer = "Answer",
                    SortOrder = 1,
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow
                },
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 3,
                    Question = "First",
                    Answer = "Answer",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();

            var result = (await new FaqRepository(context).GetActiveAsync()).ToList();

            result.Should().HaveCount(2);
            result.Select(faq => faq.Question).Should().Equal("First", "Second");
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnEmptyList_WhenThereAreNoActiveFaqs()
        {
            using var context = CreateDbContext();
            context.Faqs.Add(new TelecomSupportSystem.DAL.Entities.Faq
            {
                Question = "Inactive",
                Answer = "Answer",
                SortOrder = 1,
                IsActive = false,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var result = await new FaqRepository(context).GetActiveAsync();

            result.Should().BeEmpty();
        }
    }
}
