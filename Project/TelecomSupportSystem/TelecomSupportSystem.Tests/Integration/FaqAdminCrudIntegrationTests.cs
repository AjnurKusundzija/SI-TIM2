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
    // PB-61 / US-104: End-to-end testovi Controller → Service → Repository → DB (InMemory)
    // za admin CRUD nad FAQ stavkama.
    public class FaqAdminCrudIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static FaqController CreateController(ApplicationDbContext context)
            => new(new FaqService(new FaqRepository(context)));

        [Fact]
        public async Task CreateFaq_ShouldPersistAndBeVisibleInPublicList()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var createResult = await controller.CreateFaq(new CreateFaqDto
            {
                Question = "Kako provjeriti račun?",
                Answer = "Otvorite korisnički portal.",
                Category = "Računi",
                SortOrder = 1
            });

            createResult.Should().BeOfType<CreatedAtActionResult>();

            var publicResult = await controller.GetFaqs();
            var publicOk = publicResult.Should().BeOfType<OkObjectResult>().Subject;
            var body = publicOk.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject.ToList();
            body.Should().ContainSingle(f => f.Question == "Kako provjeriti račun?");
        }

        [Fact]
        public async Task CreateFaq_ShouldReturnBadRequest_WhenQuestionIsEmpty()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var result = await controller.CreateFaq(new CreateFaqDto
            {
                Question = "   ",
                Answer = "Neki odgovor"
            });

            result.Should().BeOfType<BadRequestObjectResult>();
            (await context.Faqs.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task CreateFaq_ShouldReturnBadRequest_WhenAnswerIsEmpty()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var result = await controller.CreateFaq(new CreateFaqDto
            {
                Question = "Validno pitanje?",
                Answer = ""
            });

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateFaq_ShouldPersistChanges()
        {
            using var context = CreateDbContext();
            context.Faqs.Add(new DAL.Entities.Faq
            {
                Question = "Staro?",
                Answer = "Stari",
                Category = "Cat",
                SortOrder = 1,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var faqId = context.Faqs.First().FaqId;
            var controller = CreateController(context);

            var updateResult = await controller.UpdateFaq(faqId, new UpdateFaqDto
            {
                Question = "Novo?",
                Answer = "Novi odgovor",
                Category = "Cat2",
                SortOrder = 9
            });

            var ok = updateResult.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeOfType<GetFaqDto>().Subject;
            dto.Question.Should().Be("Novo?");
            dto.SortOrder.Should().Be(9);

            var refreshed = await context.Faqs.AsNoTracking().FirstAsync(f => f.FaqId == faqId);
            refreshed.Question.Should().Be("Novo?");
            refreshed.Category.Should().Be("Cat2");
        }

        [Fact]
        public async Task UpdateFaq_ShouldReturnNotFound_WhenFaqMissing()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var result = await controller.UpdateFaq(999, new UpdateFaqDto
            {
                Question = "Q",
                Answer = "A"
            });

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteFaq_ShouldRemoveFromDatabase()
        {
            using var context = CreateDbContext();
            context.Faqs.Add(new DAL.Entities.Faq
            {
                Question = "Q?",
                Answer = "A",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var faqId = context.Faqs.First().FaqId;

            var controller = CreateController(context);
            var result = await controller.DeleteFaq(faqId);

            result.Should().BeOfType<NoContentResult>();
            (await context.Faqs.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task DeleteFaq_ShouldReturnNotFound_WhenFaqMissing()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var result = await controller.DeleteFaq(999);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllFaqs_ShouldIncludeInactiveEntries()
        {
            using var context = CreateDbContext();
            context.Faqs.AddRange(
                new DAL.Entities.Faq { Question = "Active?", Answer = "y", IsActive = true,  SortOrder = 1, CreatedDate = DateTime.UtcNow },
                new DAL.Entities.Faq { Question = "Inactive?", Answer = "n", IsActive = false, SortOrder = 2, CreatedDate = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var result = await controller.GetAllFaqs();
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject.ToList();
            body.Should().HaveCount(2);

            var publicResult = await controller.GetFaqs();
            var publicOk = publicResult.Should().BeOfType<OkObjectResult>().Subject;
            var publicBody = publicOk.Value.Should().BeAssignableTo<IEnumerable<GetFaqDto>>().Subject.ToList();
            publicBody.Should().HaveCount(1);
            publicBody[0].Question.Should().Be("Active?");
        }
    }
}
