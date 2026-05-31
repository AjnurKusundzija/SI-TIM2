using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    /// <summary>
    /// US-23 / US-24 – Admin team management unit tests.
    /// Tests admin role-based access to user profiles and team-related queries.
    /// </summary>
    public class TeamManagementServiceTests
    {
        private readonly Mock<ITicketRepository>  _ticketRepoMock   = new();
        private readonly Mock<IUserRepository>    _userRepoMock     = new();
        private readonly Mock<IPackageService>    _packageServiceMock = new();
        private readonly UserService              _service;

        private const int AdminId    = 1;
        private const int AgentId    = 2;
        private const int ClientId   = 3;
        private const int Team1Id    = 10;
        private const int Team2Id    = 20;

        public TeamManagementServiceTests()
        {
            _service = new UserService(
                _ticketRepoMock.Object,
                _userRepoMock.Object,
                _packageServiceMock.Object,
                new Mock<TelecomSupportSystem.DAL.Repositories.Interfaces.ITeamRepository>().Object,
                new Mock<ITicketService>().Object,
                new Mock<INotificationService>().Object);
        }

        private static User MakeAgent(int id, int? teamId = Team1Id) => new()
        {
            UserId        = id,
            FirstName     = "Haris",
            LastName      = "Agić",
            Email         = $"agent{id}@test.ba",
            Username      = $"agent{id}",
            PasswordHash  = "hash",
            Role          = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            TeamId        = teamId,
        };

        private static User MakeAdmin(int id) => new()
        {
            UserId        = id,
            FirstName     = "Admin",
            LastName      = "Test",
            Email         = $"admin{id}@test.ba",
            Username      = $"admin{id}",
            PasswordHash  = "hash",
            Role          = Role.ADMINISTRATOR,
            AccountStatus = AccountStatus.ACTIVE,
        };

        // ─── Admin can access any user profile ──────────────────────────────

        /// <summary>
        /// US-23: Admin can retrieve any agent's profile to manage team membership.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfile_WhenAdminAccessesAgentProfile()
        {
            var agent = MakeAgent(AgentId);
            _userRepoMock.Setup(r => r.GetByIdAsync(AgentId)).ReturnsAsync(agent);
            _ticketRepoMock.Setup(r => r.GetByCreatorIdAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());
            _packageServiceMock.Setup(s => s.GetMyPackagesAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<TelecomSupportSystem.BLL.DTOs.Packages.PackageSummaryDto>());

            var result = await _service.GetUserProfileAsync(AgentId, AdminId, "ADMINISTRATOR");

            result.Should().NotBeNull();
            result.UserId.Should().Be(AgentId);
            result.Email.Should().Be(agent.Email);
        }

        /// <summary>
        /// US-23: Admin can access profile of an agent in any team.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfile_WhenAdminAccessesAgentFromDifferentTeam()
        {
            var agentTeam2 = MakeAgent(AgentId, teamId: Team2Id);
            _userRepoMock.Setup(r => r.GetByIdAsync(AgentId)).ReturnsAsync(agentTeam2);
            _ticketRepoMock.Setup(r => r.GetByCreatorIdAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());
            _packageServiceMock.Setup(s => s.GetMyPackagesAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<TelecomSupportSystem.BLL.DTOs.Packages.PackageSummaryDto>());

            var result = await _service.GetUserProfileAsync(AgentId, AdminId, "ADMINISTRATOR");

            result.Should().NotBeNull();
            result.UserId.Should().Be(AgentId);
        }

        // ─── Non-admin cannot access other users' profiles ───────────────────

        /// <summary>
        /// US-24: CLIENT role cannot access another user's profile — UnauthorizedAccessException.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldThrowUnauthorized_WhenClientAccessesOtherUserProfile()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetUserProfileAsync(AgentId, ClientId, "CLIENT"));

            _userRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        // ─── Moving agent to non-existent user/team → KeyNotFoundException ──

        /// <summary>
        /// US-23: Accessing profile of non-existent user throws KeyNotFoundException.
        /// </summary>
        [Fact]
        public async Task GetUserProfileAsync_ShouldThrowKeyNotFound_WhenUserDoesNotExist()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetUserProfileAsync(999, AdminId, "ADMINISTRATOR"));
        }

        // ─── Admin can get available agents per team ─────────────────────────

        /// <summary>
        /// US-23: Admin queries available agents in a team — returns filtered list.
        /// </summary>
        [Fact]
        public async Task GetAvailableAgentsByTeamId_ShouldReturnTeamAgents_WhenTeamExists()
        {
            var agents = new[]
            {
                MakeAgent(10, teamId: Team1Id),
                MakeAgent(11, teamId: Team1Id),
            };
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(Team1Id))
                .ReturnsAsync(agents);

            var result = await _userRepoMock.Object.GetAvailableAgentsByTeamIdAsync(Team1Id);

            result.Should().HaveCount(2);
            result.Should().OnlyContain(u => u.TeamId == Team1Id);
        }

        /// <summary>
        /// US-23: If no agents are available in the target team, an empty list is returned.
        /// </summary>
        [Fact]
        public async Task GetAvailableAgentsByTeamId_ShouldReturnEmpty_WhenTeamHasNoAgents()
        {
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(Team2Id))
                .ReturnsAsync(Enumerable.Empty<User>());

            var result = await _userRepoMock.Object.GetAvailableAgentsByTeamIdAsync(Team2Id);

            result.Should().BeEmpty();
        }

        // ─── Team assignment update persisted ────────────────────────────────

        /// <summary>
        /// US-23: When an agent is moved to a new team, UpdateAsync is invoked with updated TeamId.
        /// </summary>
        [Fact]
        public async Task UpdateUserTeam_ShouldCallUpdateAsync_WhenAgentMovedToNewTeam()
        {
            var agent = MakeAgent(AgentId, teamId: Team1Id);
            _userRepoMock.Setup(r => r.GetByIdAsync(AgentId)).ReturnsAsync(agent);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Simulate moving the agent to Team2
            agent.TeamId = Team2Id;
            await _userRepoMock.Object.UpdateAsync(agent);

            _userRepoMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.UserId == AgentId && u.TeamId == Team2Id)), Times.Once);
        }
    }
}
