using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.AI
{
    // PB-70 / US-108, US-109 — kontroler MCP Admin Copilota.
    public class AdminCopilotControllerTests
    {
        private readonly Mock<IAdminCopilotService> _serviceMock = new();
        private readonly AdminCopilotController _controller;

        public AdminCopilotControllerTests()
        {
            _controller = new AdminCopilotController(_serviceMock.Object);
        }

        private void SetUser(string role)
        {
            var claims = new List<Claim> { new(ClaimTypes.Role, role) };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
        }

        private static AdminCopilotQueryRequestDto Request(string q = "Koji tim je najopterećeniji?") =>
            new() { Question = q };

        [Theory]
        [InlineData("CLIENT")]
        [InlineData("AGENT")]
        [InlineData("TECHNICIAN")]
        public async Task Query_ShouldReturnForbid_ForNonAdmin(string role)
        {
            SetUser(role);

            var result = await _controller.Query(Request(), CancellationToken.None);

            result.Should().BeOfType<ForbidResult>();
            _serviceMock.Verify(s => s.QueryAsync(It.IsAny<AdminCopilotQueryRequestDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Query_ShouldReturnOk_ForAdmin()
        {
            SetUser("ADMINISTRATOR");
            var dto = new AdminCopilotQueryResponseDto { Answer = "Sažetak", Intent = "team_workload", UsedTools = { "team.workload" } };
            _serviceMock.Setup(s => s.QueryAsync(It.IsAny<AdminCopilotQueryRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var result = await _controller.Query(Request(), CancellationToken.None);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeSameAs(dto);
        }

        [Fact]
        public async Task Query_ShouldReturnBadRequest_WhenQuestionEmpty()
        {
            SetUser("ADMINISTRATOR");

            var result = await _controller.Query(new AdminCopilotQueryRequestDto { Question = "  " }, CancellationToken.None);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Query_ShouldReturn503_WhenMcpUnavailable()
        {
            SetUser("ADMINISTRATOR");
            _serviceMock.Setup(s => s.QueryAsync(It.IsAny<AdminCopilotQueryRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new McpUnavailableException("MCP server trenutno nije dostupan."));

            var result = await _controller.Query(Request(), CancellationToken.None);

            var obj = result.Should().BeOfType<ObjectResult>().Subject;
            obj.StatusCode.Should().Be(503);
        }

        [Fact]
        public async Task Query_ShouldReturn503_WhenGroqKeyMissing()
        {
            SetUser("ADMINISTRATOR");
            _serviceMock.Setup(s => s.QueryAsync(It.IsAny<AdminCopilotQueryRequestDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("GROQ_API_KEY_2 nije konfigurisan."));

            var result = await _controller.Query(Request(), CancellationToken.None);

            var obj = result.Should().BeOfType<ObjectResult>().Subject;
            obj.StatusCode.Should().Be(503);
        }
    }
}
