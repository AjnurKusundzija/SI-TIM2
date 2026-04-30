using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class TicketControllerTests
    {
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            _ticketServiceMock = new Mock<ITicketService>();
            _controller = new TicketController(_ticketServiceMock.Object);
        }

        private void SetUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [Fact]
        public async Task GetMyTickets_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            SetUser(1);

            var tickets = new List<MyTicketDto>
            {
                new MyTicketDto
                {
                    TicketId = 1,
                    Title = "Internet problem",
                    Status = "OPEN",
                    Priority = "HIGH",
                    ProblemCategory = "INTERNET"
                }
            };

            _ticketServiceMock
                .Setup(s => s.GetMyTicketsAsync(1))
                .ReturnsAsync(tickets);

            // Act
            var result = await _controller.GetMyTickets();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(tickets);

            _ticketServiceMock.Verify(s => s.GetMyTicketsAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetMyTickets_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.GetMyTickets();

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            SetUser(7);

            var createDto = new CreateTicketDto
            {
                Subject = "Internet problem",
                Description = "Internet ne radi",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET
            };

            var createdTicket = new GetTicketDto
            {
                TicketId = 20,
                Title = createDto.Subject,
                Description = createDto.Description,
                CreatorId = 7,
                Status = "OPEN",
                Priority = "HIGH",
                ProblemCategory = "INTERNET"
            };

            _ticketServiceMock
                .Setup(s => s.CreateTicketAsync(createDto, 7))
                .ReturnsAsync(createdTicket);

            // Act
            var result = await _controller.CreateTicket(createDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(TicketController.GetMyTickets));
            createdResult.Value.Should().BeEquivalentTo(createdTicket);

            _ticketServiceMock.Verify(s => s.CreateTicketAsync(createDto, 7), Times.Once);
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
        {
            // Arrange
            var createDto = new CreateTicketDto
            {
                Subject = "Test",
                Description = "Test description",
                Priority = Priority.LOW,
                Type = ProblemCategory.INTERNET
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await _controller.CreateTicket(createDto);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}