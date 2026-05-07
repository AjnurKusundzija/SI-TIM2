using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Faq
{
    public class FaqControllerTests
    {
        private readonly Mock<IFaqService> _faqServiceMock = new();
        private readonly FaqController _controller;

        public FaqControllerTests()
        {
            _controller = new FaqController(_faqServiceMock.Object);
        }

        [Fact]
        public async Task GetFaqs_ShouldReturnOkWithFaqs()
        {
            var faqs = new List<GetFaqDto>
            {
                new()
                {
                    FaqId = 1,
                    Question = "Kako otvoriti novi tiket?",
                    Answer = "Izaberite Kreiraj tiket.",
                    Category = "Tiketi",
                    SortOrder = 1
                }
            };

            _faqServiceMock
                .Setup(service => service.GetFaqsAsync())
                .ReturnsAsync(faqs);

            var result = await _controller.GetFaqs();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(faqs);
            _faqServiceMock.Verify(service => service.GetFaqsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFaqs_ShouldReturnOkWithEmptyList_WhenThereAreNoFaqs()
        {
            _faqServiceMock
                .Setup(service => service.GetFaqsAsync())
                .ReturnsAsync(new List<GetFaqDto>());

            var result = await _controller.GetFaqs();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Empty((IEnumerable<GetFaqDto>)okResult.Value!);
        }

        [Fact]
        public void FaqController_ShouldRequireAuthorization()
        {
            var attributes = typeof(FaqController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);

            attributes.Should().NotBeEmpty();
        }
    }
}
