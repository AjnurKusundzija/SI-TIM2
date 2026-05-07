using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Performance
{
    public class FaqPerformanceTests
    {
        private const int MaxLoadTimeMilliseconds = 2000;

        [Fact]
        public async Task GetFaqs_ShouldLoadFaqListWithinTwoSeconds_InTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            for (var index = 1; index <= 100; index++)
            {
                context.Faqs.Add(new TelecomSupportSystem.DAL.Entities.Faq
                {
                    Question = $"FAQ pitanje {index}",
                    Answer = $"FAQ odgovor {index}",
                    Category = "General",
                    SortOrder = index,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();

            var controller = new FaqController(new FaqService(new FaqRepository(context)));

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.GetFaqs();
            stopwatch.Stop();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var body = okResult.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject;
            body.Should().HaveCount(100);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxLoadTimeMilliseconds);
        }
    }
}
