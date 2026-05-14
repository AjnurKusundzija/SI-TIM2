using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.Tests.TicketT
{
    public class TicketControllerForwardingTests
    {
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly TicketController _controller;

        public TicketControllerForwardingTests()
        {
            _ticketServiceMock = new Mock<ITicketService>();
            _controller = new TicketController(_ticketServiceMock.Object);
        }

        private void SetUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task GetAgentScores_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            var agentScores = new List<AgentScoreDto>
            {
                new AgentScoreDto { UserId = 2, FullName = "Agent 2", ScorePercent = 0.85 },
                new AgentScoreDto { UserId = 3, FullName = "Agent 3", ScorePercent = 0.70 }
            };

            _ticketServiceMock
                .Setup(s => s.GetAgentScoresAsync(ticketId, 1))
                .ReturnsAsync(agentScores);

            // Act
            var result = await _controller.GetAgentScores(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(agentScores);
            _ticketServiceMock.Verify(s => s.GetAgentScoresAsync(ticketId, 1), Times.Once);
        }

        [Fact]
        public async Task GetAgentScores_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.GetAgentScoresAsync(ticketId, 1))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetAgentScores(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAgentScores_ShouldReturnForbid_WhenUserIsNotAgent()
        {
            // Arrange
            SetUser(1, "CLIENT");
            int ticketId = 10;

            // Act
            var result = await _controller.GetAgentScores(ticketId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task AutoForwardTicket_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            var forwardResult = new AgentScoreDto { UserId = 2, FullName = "Agent 2", ScorePercent = 0.95 };

            _ticketServiceMock
                .Setup(s => s.AutoForwardTicketAsync(ticketId, 1))
                .ReturnsAsync(forwardResult);

            // Act
            var result = await _controller.AutoForwardTicket(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(forwardResult);
            _ticketServiceMock.Verify(s => s.AutoForwardTicketAsync(ticketId, 1), Times.Once);
        }

        [Fact]
        public async Task AutoForwardTicket_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.AutoForwardTicketAsync(ticketId, 1))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.AutoForwardTicket(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AutoForwardTicket_ShouldReturnForbid_WhenUserUnauthorized()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.AutoForwardTicketAsync(ticketId, 1))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.AutoForwardTicket(ticketId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task AutoForwardTicket_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.AutoForwardTicketAsync(ticketId, 1))
                .ThrowsAsync(new InvalidOperationException("Nema dostupnih agenata"));

            // Act
            var result = await _controller.AutoForwardTicket(ticketId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ForwardTicketToAgent_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;
            var dto = new ForwardToAgentDto { TargetAgentId = 2 };

            var forwardResult = new AgentScoreDto { UserId = 2, FullName = "Agent 2", ScorePercent = 0.90 };

            _ticketServiceMock
                .Setup(s => s.ForwardTicketToAgentAsync(ticketId, dto.TargetAgentId, 1))
                .ReturnsAsync(forwardResult);

            // Act
            var result = await _controller.ForwardTicketToAgent(ticketId, dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(forwardResult);
            _ticketServiceMock.Verify(s => s.ForwardTicketToAgentAsync(ticketId, dto.TargetAgentId, 1), Times.Once);
        }

        [Fact]
        public async Task ForwardTicketToAgent_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;
            var dto = new ForwardToAgentDto { TargetAgentId = 2 };

            _ticketServiceMock
                .Setup(s => s.ForwardTicketToAgentAsync(ticketId, dto.TargetAgentId, 1))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.ForwardTicketToAgent(ticketId, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ForwardTicketToAgent_ShouldReturnForbid_WhenUserIsNotAgent()
        {
            // Arrange
            SetUser(1, "CLIENT");
            int ticketId = 10;
            var dto = new ForwardToAgentDto { TargetAgentId = 2 };

            // Act
            var result = await _controller.ForwardTicketToAgent(ticketId, dto);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task ForwardTicketToTechnician_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            var forwardResult = new AgentScoreDto { UserId = 5, FullName = "Technician 1", ScorePercent = 0.85 };

            _ticketServiceMock
                .Setup(s => s.ForwardTicketToTechnicianAsync(ticketId, 1))
                .ReturnsAsync(forwardResult);

            // Act
            var result = await _controller.ForwardTicketToTechnician(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(forwardResult);
            _ticketServiceMock.Verify(s => s.ForwardTicketToTechnicianAsync(ticketId, 1), Times.Once);
        }

        [Fact]
        public async Task ForwardTicketToTechnician_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.ForwardTicketToTechnicianAsync(ticketId, 1))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.ForwardTicketToTechnician(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task ForwardTicketToTechnician_ShouldReturnBadRequest_WhenLocationInvalid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.ForwardTicketToTechnicianAsync(ticketId, 1))
                .ThrowsAsync(new ArgumentException("Nema tehničara na lokaciji"));

            // Act
            var result = await _controller.ForwardTicketToTechnician(ticketId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ForwardTicketToTechnician_ShouldReturnForbid_WhenUserIsNotAgent()
        {
            // Arrange
            SetUser(1, "CLIENT");
            int ticketId = 10;

            // Act
            var result = await _controller.ForwardTicketToTechnician(ticketId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }
    }
}
