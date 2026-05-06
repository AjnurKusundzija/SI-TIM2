using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class TicketDetailControllerTests
    {
        private readonly Mock<ITicketService> _serviceMock = new();
        private readonly TicketController _controller;

        public TicketDetailControllerTests()
        {
            _controller = new TicketController(_serviceMock.Object);
        }

        private void SetUser(int userId, string role = "CLIENT")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
        }

        // US-14: 200 OK kada tiket postoji i korisnik ima pristup
        [Fact]
        public async Task GetTicketById_ShouldReturnOk_WhenTicketFound()
        {
            SetUser(5, "CLIENT");
            var dto = new TicketDetailDto { TicketId = 1, Title = "Internet ne radi" };

            _serviceMock.Setup(s => s.GetTicketByIdAsync(1, 5, "CLIENT")).ReturnsAsync(dto);

            var result = await _controller.GetTicketById(1);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(dto);
        }

        // US-14: 404 kada tiket ne postoji
        [Fact]
        public async Task GetTicketById_ShouldReturnNotFound_WhenTicketMissing()
        {
            SetUser(5, "CLIENT");
            _serviceMock
                .Setup(s => s.GetTicketByIdAsync(99, 5, "CLIENT"))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.GetTicketById(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        // sigurnosno: 403 kada korisnik nema prava pristupa tiketu
        [Fact]
        public async Task GetTicketById_ShouldReturnForbid_WhenAccessDenied()
        {
            SetUser(99, "CLIENT");
            _serviceMock
                .Setup(s => s.GetTicketByIdAsync(1, 99, "CLIENT"))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.GetTicketById(1);

            result.Should().BeOfType<ForbidResult>();
        }

        // sigurnosno: 401 kada JWT claim nije prisutan
        [Fact]
        public async Task GetTicketById_ShouldReturnUnauthorized_WhenNoUserClaim()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetTicketById(1);

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
