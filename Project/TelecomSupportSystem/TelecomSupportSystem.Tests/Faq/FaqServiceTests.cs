using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Faq
{
    public class FaqServiceTests
    {
        private readonly Mock<IFaqRepository> _faqRepositoryMock = new();
        private readonly FaqService _faqService;

        public FaqServiceTests()
        {
            _faqService = new FaqService(_faqRepositoryMock.Object);
        }

        [Fact]
        public async Task GetFaqsAsync_ShouldMapFaqsToDtos()
        {
            var faqs = new List<TelecomSupportSystem.DAL.Entities.Faq>
            {
                new()
                {
                    FaqId = 7,
                    Question = "Kako resetovati ruter?",
                    Answer = "Restartujte ruter.",
                    Category = "Internet",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };

            _faqRepositoryMock
                .Setup(repository => repository.GetActiveAsync())
                .ReturnsAsync(faqs);

            var result = (await _faqService.GetFaqsAsync()).ToList();

            result.Should().HaveCount(1);
            result[0].FaqId.Should().Be(7);
            result[0].Question.Should().Be("Kako resetovati ruter?");
            result[0].Answer.Should().Be("Restartujte ruter.");
            result[0].Category.Should().Be("Internet");
            result[0].SortOrder.Should().Be(1);
            _faqRepositoryMock.Verify(repository => repository.GetActiveAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFaqsAsync_ShouldReturnEmptyList_WhenRepositoryReturnsNoFaqs()
        {
            _faqRepositoryMock
                .Setup(repository => repository.GetActiveAsync())
                .ReturnsAsync(new List<TelecomSupportSystem.DAL.Entities.Faq>());

            var result = await _faqService.GetFaqsAsync();

            result.Should().BeEmpty();
        }
    }
}
