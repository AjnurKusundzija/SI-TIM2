using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    /// <summary>
    /// US-33 / US-34 – Admin user profile management unit tests.
    /// Tests admin access to user profiles and profile DTO security.
    /// </summary>
    public class AdminUserProfileServiceTests
    {
        private readonly Mock<ITicketRepository>  _ticketRepoMock     = new();
        private readonly Mock<IUserRepository>    _userRepoMock       = new();
        private readonly Mock<IPackageService>    _packageServiceMock = new();
        private readonly UserService              _service;

        private const int AdminId   = 1;
        private const int AgentId   = 2;
        private const int ClientId  = 3;
        private const int OtherId   = 99;

        public AdminUserProfileServiceTests()
        {
            _service = new UserService(
                _ticketRepoMock.Object,
                _userRepoMock.Object,
                _packageServiceMock.Object);
        }

        private static User MakeUser(int id, Role role, string email = "") => new()
        {
            UserId        = id,
            FirstName     = "First",
            LastName      = "Last",
            Email         = string.IsNullOrEmpty(email) ? $"user{id}@test.ba" : email,
            Username      = $"user{id}",
            PasswordHash  = "bcrypt_hash_NOT_FOR_DISPLAY",
            Role          = role,
            Phone         = "+38761000000",
            Location      = Location.SARAJEVO,
            AccountStatus = AccountStatus.ACTIVE,
        };

        private void SetupUserAndEmptyDeps(User user)
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(user.UserId)).ReturnsAsync(user);
            _ticketRepoMock.Setup(r => r.GetByCreatorIdAsync(user.UserId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());
            _packageServiceMock.Setup(s => s.GetMyPackagesAsync(user.UserId))
                .ReturnsAsync(Enumerable.Empty<TelecomSupportSystem.BLL.DTOs.Packages.PackageSummaryDto>());
        }

        // ─── Admin can update/access any user profile ──────────────────────

        /// <summary>
        /// US-33: Admin can read any user's profile by userId.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfile_WhenAdminReadsAgentProfile()
        {
            var agent = MakeUser(AgentId, Role.AGENT);
            SetupUserAndEmptyDeps(agent);

            var result = await _service.GetUserProfileAsync(AgentId, AdminId, "ADMINISTRATOR");

            result.Should().NotBeNull();
            result.UserId.Should().Be(AgentId);
            result.FirstName.Should().Be("First");
            result.LastName.Should().Be("Last");
        }

        /// <summary>
        /// US-33: Admin can read any client's profile.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfile_WhenAdminReadsClientProfile()
        {
            var client = MakeUser(ClientId, Role.CLIENT);
            SetupUserAndEmptyDeps(client);

            var result = await _service.GetUserProfileAsync(ClientId, AdminId, "ADMINISTRATOR");

            result.Should().NotBeNull();
            result.UserId.Should().Be(ClientId);
            result.Role.Should().Be("CLIENT");
        }

        // ─── Response DTO must never expose password hash ──────────────────

        /// <summary>
        /// US-34: UserProfileDto must NOT contain password hash — security requirement.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldNotExposePasswordHash_WhenProfileReturned()
        {
            var agent = MakeUser(AgentId, Role.AGENT);
            SetupUserAndEmptyDeps(agent);

            var result = await _service.GetUserProfileAsync(AgentId, AdminId, "ADMINISTRATOR");

            // UserProfileDto should not contain any password-related property
            var dtoType = result.GetType();
            var passwordProp = dtoType.GetProperty("PasswordHash")
                            ?? dtoType.GetProperty("Password")
                            ?? dtoType.GetProperty("Hash");

            passwordProp.Should().BeNull("UserProfileDto must not expose password hash");
        }

        // ─── Non-admin role cannot access other users' profiles ────────────

        /// <summary>
        /// US-34: CLIENT cannot access another user's profile — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherProfile()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetUserProfileAsync(AgentId, ClientId, "CLIENT"));

            _userRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        // ─── Update email (admin changes user email) ───────────────────────

        /// <summary>
        /// US-33: Admin can update a user's email via UserService.UpdateEmailAsync.
        /// </summary>
        [Fact]
        public async Task UpdateEmailAsync_ShouldUpdateAndCallRepo_WhenNewEmailIsUnique()
        {
            var agent = MakeUser(AgentId, Role.AGENT);
            _userRepoMock.Setup(r => r.GetByIdAsync(AgentId)).ReturnsAsync(agent);
            _userRepoMock.Setup(r => r.GetByEmailAsync("new@test.ba")).ReturnsAsync((User?)null);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _service.UpdateEmailAsync(AgentId, new UpdateEmailDto { Email = "new@test.ba" });

            _userRepoMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.UserId == AgentId && u.Email == "new@test.ba")), Times.Once);
        }

        /// <summary>
        /// US-33: Email update fails if email already taken — InvalidOperationException.
        /// </summary>
        [Fact]
        public async Task UpdateEmailAsync_ShouldThrowInvalidOperation_WhenEmailAlreadyTaken()
        {
            var agent        = MakeUser(AgentId, Role.AGENT, "agent@test.ba");
            var conflicting  = MakeUser(OtherId, Role.CLIENT, "taken@test.ba");
            _userRepoMock.Setup(r => r.GetByIdAsync(AgentId)).ReturnsAsync(agent);
            _userRepoMock.Setup(r => r.GetByEmailAsync("taken@test.ba")).ReturnsAsync(conflicting);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.UpdateEmailAsync(AgentId, new UpdateEmailDto { Email = "taken@test.ba" }));

            _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// US-33: Update fails if user does not exist — KeyNotFoundException.
        /// </summary>
        [Fact]
        public async Task UpdateEmailAsync_ShouldThrowKeyNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateEmailAsync(999, new UpdateEmailDto { Email = "any@test.ba" }));
        }
    }
}
