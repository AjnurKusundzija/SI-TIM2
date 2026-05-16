using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Ratings;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Rating
{
    public class RatingControllerTests
    {
        private readonly Mock<IRatingService> _serviceMock = new();
        private readonly TicketRatingController _controller;

        public RatingControllerTests()
        {
            _controller = new TicketRatingController(_serviceMock.Object);
        }

        private void SetUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            };
        }

        // GET: 200 OK kada agent dohvati postojeću ocjenu
        [Fact]
        public async Task GetRating_ShouldReturnOk_WhenRatingExists()
        {
            SetUser(10, "AGENT");
            var ratingDto = new RatingDto { RatingValue = 4, RatingComment = "Odlično", RatingDate = DateTime.UtcNow, UserId = 5 };
            _serviceMock.Setup(s => s.GetRatingForTicketAsync(1, 10, "AGENT")).ReturnsAsync(ratingDto);

            var result = await _controller.GetRating(1);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(ratingDto);
        }

        // GET: 204 No Content kada ocjena ne postoji
        [Fact]
        public async Task GetRating_ShouldReturnNoContent_WhenNoRatingExists()
        {
            SetUser(5, "CLIENT");
            _serviceMock.Setup(s => s.GetRatingForTicketAsync(1, 5, "CLIENT")).ReturnsAsync((RatingDto?)null);

            var result = await _controller.GetRating(1);

            result.Should().BeOfType<NoContentResult>();
        }

        // GET: 403 Forbidden za TECHNICIAN (nema pristup)
        [Fact]
        public async Task GetRating_ShouldReturnForbid_WhenTechnicianRequests()
        {
            SetUser(20, "TECHNICIAN");
            _serviceMock.Setup(s => s.GetRatingForTicketAsync(1, 20, "TECHNICIAN"))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.GetRating(1);

            result.Should().BeOfType<ForbidResult>();
        }

        // GET: 404 kada tiket ne postoji
        [Fact]
        public async Task GetRating_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            SetUser(5, "CLIENT");
            _serviceMock.Setup(s => s.GetRatingForTicketAsync(99, 5, "CLIENT"))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.GetRating(99);

            result.Should().BeOfType<NotFoundResult>();
        }

        // POST: 201 Created — klijent uspješno ocjenjuje tiket
        [Fact]
        public async Task CreateRating_ShouldReturnCreated_WhenClientRatesClosedTicket()
        {
            SetUser(5, "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 5, RatingComment = "Brzo i profesionalno!" };
            var ratingDto = new RatingDto { RatingValue = 5, RatingComment = "Brzo i profesionalno!", RatingDate = DateTime.UtcNow, UserId = 5 };
            _serviceMock.Setup(s => s.CreateRatingAsync(1, 5, "CLIENT", dto)).ReturnsAsync(ratingDto);

            var result = await _controller.CreateRating(1, dto);

            result.Should().BeOfType<CreatedAtActionResult>();
        }

        // POST: 403 Forbidden kada AGENT pokušava ocijeniti
        [Fact]
        public async Task CreateRating_ShouldReturnForbid_WhenAgentTriesToRate()
        {
            SetUser(10, "AGENT");
            var dto = new CreateRatingDto { RatingValue = 3 };
            _serviceMock.Setup(s => s.CreateRatingAsync(1, 10, "AGENT", dto))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateRating(1, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        // POST: 409 Conflict kada tiket već ima ocjenu
        [Fact]
        public async Task CreateRating_ShouldReturnConflict_WhenTicketAlreadyRated()
        {
            SetUser(5, "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 4 };
            _serviceMock.Setup(s => s.CreateRatingAsync(1, 5, "CLIENT", dto))
                .ThrowsAsync(new InvalidOperationException("This ticket has already been rated."));

            var result = await _controller.CreateRating(1, dto);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        // POST: 409 Conflict kada tiket nije zatvoren
        [Fact]
        public async Task CreateRating_ShouldReturnConflict_WhenTicketIsNotClosed()
        {
            SetUser(5, "CLIENT");
            var dto = new CreateRatingDto { RatingValue = 4 };
            _serviceMock.Setup(s => s.CreateRatingAsync(1, 5, "CLIENT", dto))
                .ThrowsAsync(new InvalidOperationException("Only closed tickets can be rated."));

            var result = await _controller.CreateRating(1, dto);

            result.Should().BeOfType<ConflictObjectResult>();
        }

        // POST: 401 Unauthorized bez JWT
        [Fact]
        public async Task CreateRating_ShouldReturnUnauthorized_WhenNoJwt()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
            var dto = new CreateRatingDto { RatingValue = 4 };

            var result = await _controller.CreateRating(1, dto);

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
