using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-51 — UserController unit testovi (US-73, US-74, US-75, US-89)
    public class UserAccountManagementControllerTests
    {
        private readonly Mock<IUserService> _userService = new();
        private readonly UserController _controller;

        public UserAccountManagementControllerTests()
        {
            _controller = new UserController(_userService.Object);
        }

        private void SetUser(int userId, string role)
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Role, role),
                    }, "Test"))
                }
            };
        }

        private static CreateUserDto MakeCreateDto(Role role = Role.CLIENT) => new()
        {
            FirstName = "Ime",
            LastName = "Prezime",
            Email = "novi@test.ba",
            Phone = "061123456",
            Password = "StrongPass!23",
            Role = role,
            Location = Location.SARAJEVO,
        };

        // ── US-73: CreateUser ──────────────────────────────────────────────────

        [Fact]
        public async Task CreateUser_ShouldReturnOk_WhenAdminAndValidDto()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.CreateUserAsync(
                    It.IsAny<CreateUserDto>(),
                    "ADMINISTRATOR",
                    It.IsAny<int?>(),
                    It.IsAny<string?>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateUser(MakeCreateDto());

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnForbid_WhenAgentTriesToCreate()
        {
            SetUser(2, "AGENT");
            _userService.Setup(s => s.CreateUserAsync(
                    It.IsAny<CreateUserDto>(),
                    "AGENT",
                    It.IsAny<int?>(),
                    It.IsAny<string?>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.CreateUser(MakeCreateDto());

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnUnauthorized_WhenNoRoleClaim()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
            var result = await _controller.CreateUser(MakeCreateDto());

            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnConflict_WhenEmailAlreadyTaken()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.CreateUserAsync(
                    It.IsAny<CreateUserDto>(),
                    "ADMINISTRATOR",
                    It.IsAny<int?>(),
                    It.IsAny<string?>()))
                .ThrowsAsync(new InvalidOperationException("Email zauzet."));

            var result = await _controller.CreateUser(MakeCreateDto());

            result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            SetUser(1, "ADMINISTRATOR");
            _controller.ModelState.AddModelError("Email", "Email je obavezan.");

            var result = await _controller.CreateUser(new CreateUserDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ── US-74: UpdateUserDetails ───────────────────────────────────────────

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnOk_WhenAdminEdits()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.UpdateUserDetailsAsync(
                    2,
                    It.IsAny<UpdateUserDetailsDto>(),
                    "ADMINISTRATOR",
                    It.IsAny<int?>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateUserDetails(2, new UpdateUserDetailsDto
            {
                FirstName = "A",
                LastName = "B",
            });

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.UpdateUserDetailsAsync(
                    404,
                    It.IsAny<UpdateUserDetailsDto>(),
                    "ADMINISTRATOR",
                    It.IsAny<int?>()))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateUserDetails(404, new UpdateUserDetailsDto
            {
                FirstName = "A",
                LastName = "B",
            });

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnForbid_WhenServiceThrowsUnauthorized()
        {
            SetUser(99, "AGENT");
            _userService.Setup(s => s.UpdateUserDetailsAsync(
                    5,
                    It.IsAny<UpdateUserDetailsDto>(),
                    "AGENT",
                    It.IsAny<int?>()))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.UpdateUserDetails(5, new UpdateUserDetailsDto
            {
                FirstName = "A",
                LastName = "B",
            });

            result.Should().BeOfType<ForbidResult>();
        }

        // ── US-75 / US-89: Deactivate / Reactivate ─────────────────────────────

        [Fact]
        public async Task DeactivateUser_ShouldReturnOk_WhenAdminDeactivatesClient()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.ChangeUserStatusAsync(20, false, "ADMINISTRATOR", 1))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeactivateUser(20);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnBadRequest_WhenServiceThrowsInvalidOperation()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.ChangeUserStatusAsync(1, false, "ADMINISTRATOR", 1))
                .ThrowsAsync(new InvalidOperationException("Nije moguće deaktivirati vlastiti nalog."));

            var result = await _controller.DeactivateUser(1);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnForbid_WhenAgentDeactivatesAgent()
        {
            SetUser(2, "AGENT");
            _userService.Setup(s => s.ChangeUserStatusAsync(3, false, "AGENT", 2))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.DeactivateUser(3);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnNotFound_WhenUserMissing()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.ChangeUserStatusAsync(999, false, "ADMINISTRATOR", 1))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.DeactivateUser(999);

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ReactivateUser_ShouldReturnOk_WhenAdminReactivates()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.ChangeUserStatusAsync(7, true, "ADMINISTRATOR", 1))
                .Returns(Task.CompletedTask);

            var result = await _controller.ReactivateUser(7);

            result.Should().BeOfType<OkObjectResult>();
        }

        // ── US-74 / US-89: GetUsersList ────────────────────────────────────────

        [Fact]
        public async Task GetUsersList_ShouldReturnOk_WhenAdminCalls()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.GetUsersPaginatedAsync("ADMINISTRATOR", null, null, null, null, 1, 10))
                .ReturnsAsync(new UserListDto { Users = new List<UserListItemDto>(), TotalCount = 0, Page = 1, PageSize = 10 });

            var result = await _controller.GetUsersList(null, null, null, null, 1, 10);

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetUsersList_ShouldReturnForbid_WhenClientCalls()
        {
            SetUser(1, "CLIENT");
            _userService.Setup(s => s.GetUsersPaginatedAsync("CLIENT", null, null, null, null, 1, 10))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.GetUsersList(null, null, null, null, 1, 10);

            result.Should().BeOfType<ForbidResult>();
        }

        // ── US-89: agent teams (samo admin) ────────────────────────────────────

        [Fact]
        public async Task GetAgentTeams_ShouldReturnForbid_WhenAgentCalls()
        {
            SetUser(1, "AGENT");

            var result = await _controller.GetAgentTeams();

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task GetAgentTeams_ShouldReturnOk_WhenAdminCalls()
        {
            SetUser(1, "ADMINISTRATOR");
            _userService.Setup(s => s.GetAgentTeamsAsync())
                .ReturnsAsync(Enumerable.Empty<TelecomSupportSystem.BLL.DTOs.Teams.TeamDto>());

            var result = await _controller.GetAgentTeams();

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
