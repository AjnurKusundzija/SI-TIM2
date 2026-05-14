using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.Tests.TicketT
{
    public class TicketControllerClosureTests
    {
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly TicketController _controller;

        public TicketControllerClosureTests()
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
        public async Task RequestClosure_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.RequestClosureAsync(ticketId, 1, "AGENT"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RequestClosure(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.RequestClosureAsync(ticketId, 1, "AGENT"), Times.Once);
        }

        [Fact]
        public async Task RequestClosure_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.RequestClosureAsync(ticketId, 1, "AGENT"))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.RequestClosure(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RequestClosure_ShouldReturnForbid_WhenUserUnauthorized()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.RequestClosureAsync(ticketId, 1, "AGENT"))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.RequestClosure(ticketId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task RequestClosure_ShouldReturnBadRequest_WhenInvalidOperation()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.RequestClosureAsync(ticketId, 1, "AGENT"))
                .ThrowsAsync(new InvalidOperationException("Tiket je već zatvoren"));

            // Act
            var result = await _controller.RequestClosure(ticketId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        }

        [Fact]
        public async Task AcceptClosure_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(5, "CLIENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.AcceptClosureAsync(ticketId, 5))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AcceptClosure(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.AcceptClosureAsync(ticketId, 5), Times.Once);
        }

        [Fact]
        public async Task AcceptClosure_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(5, "CLIENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.AcceptClosureAsync(ticketId, 5))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.AcceptClosure(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RejectClosure_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(5, "CLIENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.RejectClosureAsync(ticketId, 5))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RejectClosure(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.RejectClosureAsync(ticketId, 5), Times.Once);
        }

        [Fact]
        public async Task RejectClosure_ShouldReturnForbid_WhenUserUnauthorized()
        {
            // Arrange
            SetUser(5, "CLIENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.RejectClosureAsync(ticketId, 5))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.RejectClosure(ticketId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task ForceClose_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.ForceCloseAsync(ticketId, 1, "AGENT"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ForceClose(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.ForceCloseAsync(ticketId, 1, "AGENT"), Times.Once);
        }

        [Fact]
        public async Task ForceClose_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;

            _ticketServiceMock
                .Setup(s => s.ForceCloseAsync(ticketId, 1, "AGENT"))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.ForceClose(ticketId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CloseTicket_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;

            _ticketServiceMock
                .Setup(s => s.CloseTicketAsync(ticketId, 1, "AGENT"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CloseTicket(ticketId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.CloseTicketAsync(ticketId, 1, "AGENT"), Times.Once);
        }

        [Fact]
        public async Task UpdateInternalPriority_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 10;
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.HIGH };

            _ticketServiceMock
                .Setup(s => s.UpdateInternalPriorityAsync(ticketId, dto.Priority, 1, "AGENT"))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateInternalPriority(ticketId, dto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            _ticketServiceMock.Verify(s => s.UpdateInternalPriorityAsync(ticketId, dto.Priority, 1, "AGENT"), Times.Once);
        }

        [Fact]
        public async Task UpdateInternalPriority_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            // Arrange
            SetUser(1, "AGENT");
            int ticketId = 999;
            var dto = new UpdateInternalPriorityDto { Priority = InternalPriority.HIGH };

            _ticketServiceMock
                .Setup(s => s.UpdateInternalPriorityAsync(ticketId, dto.Priority, 1, "AGENT"))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdateInternalPriority(ticketId, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
