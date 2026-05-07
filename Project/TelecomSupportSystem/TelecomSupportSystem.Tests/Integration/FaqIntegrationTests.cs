using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Integration
{
    public class FaqIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetFaqs_ShouldReturnValidApiResponse_WithActiveFaqList()
        {
            using var context = CreateDbContext();
            context.Faqs.AddRange(
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 1,
                    Question = "Kako resetovati ruter?",
                    Answer = "Iskljucite ruter 30 sekundi i ponovo ga ukljucite.",
                    Category = "Internet",
                    SortOrder = 2,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 2,
                    Question = "Kako provjeriti racun?",
                    Answer = "Otvorite korisnicki portal i odaberite sekciju Racuni.",
                    Category = "Racuni",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new TelecomSupportSystem.DAL.Entities.Faq
                {
                    FaqId = 3,
                    Question = "Neaktivno pitanje",
                    Answer = "Ovaj odgovor se ne prikazuje.",
                    Category = "Arhiva",
                    SortOrder = 3,
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.GetFaqs();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var body = okResult.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject.ToList();
            body.Should().HaveCount(2);
            body.Select(faq => faq.Question).Should().Equal("Kako provjeriti racun?", "Kako resetovati ruter?");
            body.Should().OnlyContain(faq => !string.IsNullOrWhiteSpace(faq.Answer));
        }

        [Fact]
        public async Task GetFaqs_ShouldReturnEmptyList_WhenNoActiveFaqContentExists()
        {
            using var context = CreateDbContext();
            context.Faqs.Add(new TelecomSupportSystem.DAL.Entities.Faq
            {
                Question = "Arhivirano pitanje",
                Answer = "Arhivirani odgovor.",
                Category = "Arhiva",
                SortOrder = 1,
                IsActive = false,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.GetFaqs();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var body = okResult.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject;
            body.Should().BeEmpty();
        }

        private static FaqController CreateController(ApplicationDbContext context)
        {
            var repository = new FaqRepository(context);
            var service = new FaqService(repository);

            return new FaqController(service);
        }
    }
}
