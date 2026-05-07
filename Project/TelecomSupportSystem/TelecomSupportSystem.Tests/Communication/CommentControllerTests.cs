using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Communication
{
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _serviceMock = new();
        private readonly CommentController _controller;
        private readonly Mock<IHubContext<ChatHub>> _hubContextMock;

        public CommentControllerTests()
        {
            _controller = new CommentController(_serviceMock.Object);

            _hubContextMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var groupMock = new Mock<IClientProxy>();
            _hubContextMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(groupMock.Object);
            groupMock.Setup(g => g.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);
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

        // US-15: 200 OK sa listom komentara kada je korisnik autorizovan
        [Fact]
        public async Task GetCommentsForTicket_ShouldReturnOk_WhenAuthorized()
        {
            SetUser(5, "CLIENT");
            var comments = new List<CommentDto>
            {
                new() { CommentId = 1, Content = "Javio sam se agentom.", AuthorId = 5 }
            };
            _serviceMock.Setup(s => s.GetCommentsForTicketAsync(1, 5, "CLIENT")).ReturnsAsync(comments);

            var result = await _controller.GetCommentsForTicket(1);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(comments);
        }

        // sigurnosno: 401 kada JWT claim nije prisutan
        [Fact]
        public async Task GetCommentsForTicket_ShouldReturnUnauthorized_WhenNoUserClaim()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.GetCommentsForTicket(1);

            result.Should().BeOfType<UnauthorizedResult>();
        }

        // US-20: 400 Bad Request kada sadržaj poruke je prazan
        [Fact]
        public async Task AddComment_ShouldReturnBadRequest_WhenContentIsEmpty()
        {
            SetUser(5, "CLIENT");
            var request = new CommentController.CreateCommentRequest { Content = "" };

            var result = await _controller.AddComment(1, request, _hubContextMock.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // sigurnosno: 401 kada JWT claim nije prisutan pri slanju poruke
        [Fact]
        public async Task AddComment_ShouldReturnUnauthorized_WhenNoUserClaim()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            var request = new CommentController.CreateCommentRequest { Content = "Poruka" };

            var result = await _controller.AddComment(1, request, _hubContextMock.Object);

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
