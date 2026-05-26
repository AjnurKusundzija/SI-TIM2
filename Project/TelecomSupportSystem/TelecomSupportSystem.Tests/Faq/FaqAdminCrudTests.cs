using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Faq
{
    // PB-61 / US-104: Admin CRUD nad FAQ stavkama.
    // Pokriva: validaciju praznih polja, mapiranje DTO → entity, autorizaciju controller-a,
    // ne-postojeću FAQ stavku i admin-only endpointe.
    public class FaqAdminCrudTests
    {
        private readonly Mock<IFaqRepository> _faqRepositoryMock = new();
        private readonly FaqService _faqService;

        public FaqAdminCrudTests()
        {
            _faqService = new FaqService(_faqRepositoryMock.Object);
        }

        // ─── Service: CREATE ─────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateFaqAsync_ShouldThrow_WhenQuestionIsEmpty()
        {
            var dto = new CreateFaqDto { Question = "   ", Answer = "Validan odgovor" };

            var act = () => _faqService.CreateFaqAsync(dto);

            (await act.Should().ThrowAsync<ArgumentException>())
                .WithMessage(FaqService.QuestionRequiredMessage);
            _faqRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<DAL.Entities.Faq>()), Times.Never);
        }

        [Fact]
        public async Task CreateFaqAsync_ShouldThrow_WhenAnswerIsEmpty()
        {
            var dto = new CreateFaqDto { Question = "Validno pitanje?", Answer = "" };

            var act = () => _faqService.CreateFaqAsync(dto);

            (await act.Should().ThrowAsync<ArgumentException>())
                .WithMessage(FaqService.AnswerRequiredMessage);
        }

        [Fact]
        public async Task CreateFaqAsync_ShouldPersistTrimmedFaq_WhenInputIsValid()
        {
            _faqRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<DAL.Entities.Faq>()))
                .ReturnsAsync((DAL.Entities.Faq f) => { f.FaqId = 42; return f; });

            var dto = new CreateFaqDto
            {
                Question = "  Kako resetovati ruter?  ",
                Answer = "  Isključite ga 30 sekundi.  ",
                Category = "  Internet  ",
                SortOrder = 5
            };

            var result = await _faqService.CreateFaqAsync(dto);

            result.FaqId.Should().Be(42);
            result.Question.Should().Be("Kako resetovati ruter?");
            result.Answer.Should().Be("Isključite ga 30 sekundi.");
            result.Category.Should().Be("Internet");
            result.SortOrder.Should().Be(5);
            _faqRepositoryMock.Verify(r => r.CreateAsync(It.Is<DAL.Entities.Faq>(
                f => f.Question == "Kako resetovati ruter?"
                  && f.Answer == "Isključite ga 30 sekundi."
                  && f.Category == "Internet"
                  && f.IsActive == true)), Times.Once);
        }

        // ─── Service: UPDATE ─────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateFaqAsync_ShouldThrowKeyNotFound_WhenFaqDoesNotExist()
        {
            _faqRepositoryMock.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((DAL.Entities.Faq?)null);

            var act = () => _faqService.UpdateFaqAsync(99, new UpdateFaqDto { Question = "Q", Answer = "A" });

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateFaqAsync_ShouldThrow_WhenQuestionIsEmpty()
        {
            var dto = new UpdateFaqDto { Question = "", Answer = "ok" };

            var act = () => _faqService.UpdateFaqAsync(1, dto);

            (await act.Should().ThrowAsync<ArgumentException>())
                .WithMessage(FaqService.QuestionRequiredMessage);
        }

        [Fact]
        public async Task UpdateFaqAsync_ShouldPersistChanges_WhenInputIsValid()
        {
            var existing = new DAL.Entities.Faq
            {
                FaqId = 7,
                Question = "Staro pitanje",
                Answer = "Stari odgovor",
                Category = "Stari",
                SortOrder = 1,
                IsActive = true
            };
            _faqRepositoryMock.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);

            var dto = new UpdateFaqDto
            {
                Question = "Novo pitanje?",
                Answer = "Novi odgovor.",
                Category = "Novi",
                SortOrder = 3
            };

            var result = await _faqService.UpdateFaqAsync(7, dto);

            result.Question.Should().Be("Novo pitanje?");
            result.Answer.Should().Be("Novi odgovor.");
            result.Category.Should().Be("Novi");
            result.SortOrder.Should().Be(3);
            _faqRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        // ─── Service: DELETE ─────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteFaqAsync_ShouldThrowKeyNotFound_WhenFaqDoesNotExist()
        {
            _faqRepositoryMock.Setup(r => r.GetByIdAsync(123))
                .ReturnsAsync((DAL.Entities.Faq?)null);

            var act = () => _faqService.DeleteFaqAsync(123);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteFaqAsync_ShouldRemove_WhenFaqExists()
        {
            var existing = new DAL.Entities.Faq { FaqId = 5, Question = "Q", Answer = "A" };
            _faqRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);

            await _faqService.DeleteFaqAsync(5);

            _faqRepositoryMock.Verify(r => r.DeleteAsync(existing), Times.Once);
        }

        // ─── Service: GET ALL ────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllFaqsAsync_ShouldReturnInactiveAndActive()
        {
            var faqs = new List<DAL.Entities.Faq>
            {
                new() { FaqId = 1, Question = "A?", Answer = "1", IsActive = true,  SortOrder = 1 },
                new() { FaqId = 2, Question = "B?", Answer = "2", IsActive = false, SortOrder = 2 },
            };
            _faqRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(faqs);

            var result = (await _faqService.GetAllFaqsAsync()).ToList();

            result.Should().HaveCount(2);
            result.Select(f => f.FaqId).Should().Equal(1, 2);
        }

        // ─── Controller: authorization metadata ──────────────────────────────────────

        [Theory]
        [InlineData(nameof(FaqController.CreateFaq))]
        [InlineData(nameof(FaqController.UpdateFaq))]
        [InlineData(nameof(FaqController.DeleteFaq))]
        [InlineData(nameof(FaqController.GetAllFaqs))]
        public void AdminEndpoints_ShouldRequireAdministratorRole(string methodName)
        {
            var method = typeof(FaqController).GetMethod(methodName);
            method.Should().NotBeNull();

            var authorize = method!
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
                .Cast<AuthorizeAttribute>()
                .ToList();

            authorize.Should().NotBeEmpty();
            authorize.Should().Contain(a => a.Roles == "ADMINISTRATOR");
        }

        [Fact]
        public void GetFaqs_ShouldNotRequireAdministratorRole()
        {
            var method = typeof(FaqController).GetMethod(nameof(FaqController.GetFaqs));
            method.Should().NotBeNull();

            // GET / nema role-specific [Authorize] — pristup imaju svi prijavljeni korisnici
            var authorizeWithRoles = method!
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
                .Cast<AuthorizeAttribute>()
                .Where(a => !string.IsNullOrEmpty(a.Roles))
                .ToList();

            authorizeWithRoles.Should().BeEmpty();
        }

        // ─── Controller: behavior via mocked service ─────────────────────────────────

        [Fact]
        public async Task CreateFaq_ShouldReturnBadRequest_WhenServiceThrowsArgumentException()
        {
            var serviceMock = new Mock<IFaqService>();
            serviceMock.Setup(s => s.CreateFaqAsync(It.IsAny<CreateFaqDto>()))
                .ThrowsAsync(new ArgumentException("Pitanje ne smije biti prazno."));

            var controller = new FaqController(serviceMock.Object);

            var result = await controller.CreateFaq(new CreateFaqDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateFaq_ShouldReturnNotFound_WhenServiceThrowsKeyNotFound()
        {
            var serviceMock = new Mock<IFaqService>();
            serviceMock.Setup(s => s.UpdateFaqAsync(99, It.IsAny<UpdateFaqDto>()))
                .ThrowsAsync(new KeyNotFoundException());

            var controller = new FaqController(serviceMock.Object);

            var result = await controller.UpdateFaq(99, new UpdateFaqDto { Question = "Q", Answer = "A" });

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteFaq_ShouldReturnNoContent_OnSuccess()
        {
            var serviceMock = new Mock<IFaqService>();
            serviceMock.Setup(s => s.DeleteFaqAsync(7)).Returns(Task.CompletedTask);

            var controller = new FaqController(serviceMock.Object);

            var result = await controller.DeleteFaq(7);

            result.Should().BeOfType<NoContentResult>();
            serviceMock.Verify(s => s.DeleteFaqAsync(7), Times.Once);
        }
    }
}
