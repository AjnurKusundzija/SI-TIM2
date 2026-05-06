using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    public class AllTicketsControllerTests
    {
        private readonly Mock<ITicketService> _serviceMock = new();
        private readonly TicketController _controller;

        public AllTicketsControllerTests()
        {
            _controller = new TicketController(_serviceMock.Object);
        }

        private void SetUser(int userId, string role)
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

        // PB-32: agent dobija 200 OK sa listom svih tiketa
        [Fact]
        public async Task GetAllTickets_ShouldReturnOk_WhenAgentCalls()
        {
            SetUser(10, "AGENT");
            var tickets = new List<MyTicketDto>
            {
                new() { TicketId = 1, Title = "Internet ne radi", Status = "OPEN" },
                new() { TicketId = 2, Title = "TV problem", Status = "CLOSED" },
            };
            _serviceMock.Setup(s => s.GetAllTicketsAsync(10, "AGENT", false)).ReturnsAsync(tickets);

            var result = await _controller.GetAllTickets();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(tickets);
        }

        // sigurnosno: klijent ne može pristupiti svim tiketima — dobija 403
        [Fact]
        public async Task GetAllTickets_ShouldReturnForbid_WhenClientCalls()
        {
            SetUser(5, "CLIENT");
            _serviceMock
                .Setup(s => s.GetAllTicketsAsync(5, "CLIENT", false))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.GetAllTickets();

            result.Should().BeOfType<ForbidResult>();
        }

        // sigurnosno: 401 kada JWT claim nije prisutan
        [Fact]
        public async Task GetAllTickets_ShouldReturnUnauthorized_WhenNoUserClaim()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetAllTickets();

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
