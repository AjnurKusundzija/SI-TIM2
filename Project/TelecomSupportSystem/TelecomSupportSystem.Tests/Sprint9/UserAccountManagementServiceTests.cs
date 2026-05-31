using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-51 — Upravljanje korisničkim nalozima (US-73, US-74, US-75, US-89)
    // Unit testovi nad UserService bez izmjena produkcijskog koda.
    public class UserAccountManagementServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepo = new();
        private readonly Mock<IUserRepository> _userRepo = new();
        private readonly Mock<IPackageService> _packageService = new();
        private readonly Mock<ITeamRepository> _teamRepo = new();
        private readonly UserService _service;

        public UserAccountManagementServiceTests()
        {
            _service = new UserService(
                _ticketRepo.Object,
                _userRepo.Object,
                _packageService.Object,
                _teamRepo.Object,
                new Mock<ITicketService>().Object,
                new Mock<INotificationService>().Object);
        }

        private static User MakeUser(int id, Role role, AccountStatus status = AccountStatus.ACTIVE) => new()
        {
            UserId = id,
            FirstName = $"Name{id}",
            LastName = $"Last{id}",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            Phone = "061111111",
            Location = Location.SARAJEVO,
            Role = role,
            AccountStatus = status,
        };

        private static CreateUserDto MakeCreateDto(Role role, string email = "new.user@test.ba", int? teamId = null) => new()
        {
            FirstName = "Novi",
            LastName = "Korisnik",
            Email = email,
            Phone = "061123456",
            Password = "StrongPass!23",
            Role = role,
            Location = Location.SARAJEVO,
            TeamId = teamId,
        };

        // ── US-73: Kreiranje korisničkih naloga ─────────────────────────────────

        [Theory]
        [InlineData(Role.CLIENT)]
        [InlineData(Role.AGENT)]
        [InlineData(Role.TECHNICIAN)]
        public async Task CreateUserAsync_ShouldCreate_WhenAdminCreatesAllowedRoles(Role role)
        {
            _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepo.Setup(r => r.CreateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var dto = MakeCreateDto(role, $"role.{role}@test.ba", role == Role.AGENT ? 1 : null);
            await _service.CreateUserAsync(dto, "ADMINISTRATOR");

            _userRepo.Verify(r => r.CreateAsync(It.Is<User>(u =>
                u.Role == role
                && u.AccountStatus == AccountStatus.ACTIVE
                && !string.IsNullOrEmpty(u.PasswordHash)
                && u.PasswordHash != dto.Password)), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldHashPassword_NeverStoringPlaintext()
        {
            _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            User? captured = null;
            _userRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => captured = u)
                .Returns(Task.CompletedTask);

            var dto = MakeCreateDto(Role.CLIENT, "client.hash@test.ba");
            await _service.CreateUserAsync(dto, "ADMINISTRATOR");

            captured.Should().NotBeNull();
            captured!.PasswordHash.Should().NotBe(dto.Password);
            BCrypt.Net.BCrypt.Verify(dto.Password, captured.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowInvalidOperation_WhenEmailAlreadyExists()
        {
            var existing = MakeUser(5, Role.CLIENT);
            _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(existing);

            var dto = MakeCreateDto(Role.CLIENT, existing.Email);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateUserAsync(dto, "ADMINISTRATOR"));
            _userRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData("AGENT")]
        [InlineData("CLIENT")]
        [InlineData("TECHNICIAN")]
        public async Task CreateUserAsync_ShouldThrowUnauthorized_WhenNonAdminCallsService(string role)
        {
            var dto = MakeCreateDto(Role.CLIENT);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CreateUserAsync(dto, role));
            _userRepo.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAssignAvailableAvailability_ForAgentAndTechnician()
        {
            _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            User? captured = null;
            _userRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .Callback<User>(u => captured = u)
                .Returns(Task.CompletedTask);

            await _service.CreateUserAsync(MakeCreateDto(Role.AGENT, "agent.av@test.ba", 7), "ADMINISTRATOR");
            captured!.AvailabilityStatus.Should().Be(AvailabilityStatus.AVAILABLE);
            captured.TeamId.Should().Be(7);
        }

        // ── US-74: Uređivanje podataka postojećih korisnika ─────────────────────

        [Fact]
        public async Task UpdateUserDetailsAsync_ShouldUpdateNamePhoneLocation_WhenAdminEditsAgent()
        {
            var target = MakeUser(2, Role.AGENT);
            _userRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(target);
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var dto = new UpdateUserDetailsDto
            {
                FirstName = "Novo",
                LastName = "Prezime",
                Phone = "061888888",
                Location = Location.TUZLA,
            };

            await _service.UpdateUserDetailsAsync(2, dto, "ADMINISTRATOR");

            _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.FirstName == "Novo"
                && u.LastName == "Prezime"
                && u.Phone == "061888888"
                && u.Location == Location.TUZLA)), Times.Once);
        }

        [Fact]
        public async Task UpdateUserDetailsAsync_ShouldNotChangeRole_EvenIfTargetWasAdmin()
        {
            var target = MakeUser(3, Role.CLIENT);
            _userRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(target);
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _service.UpdateUserDetailsAsync(3, new UpdateUserDetailsDto
            {
                FirstName = "X",
                LastName = "Y",
            }, "ADMINISTRATOR");

            _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Role == Role.CLIENT)), Times.Once);
        }

        [Fact]
        public async Task UpdateUserDetailsAsync_ShouldThrowKeyNotFound_WhenUserMissing()
        {
            _userRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateUserDetailsAsync(404, new UpdateUserDetailsDto
                {
                    FirstName = "A",
                    LastName = "B",
                }, "ADMINISTRATOR"));
        }

        [Fact]
        public async Task UpdateUserDetailsAsync_ShouldThrowUnauthorized_WhenAgentEditsAgent()
        {
            var agent = MakeUser(9, Role.AGENT);
            _userRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(agent);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateUserDetailsAsync(9, new UpdateUserDetailsDto
                {
                    FirstName = "X",
                    LastName = "Y",
                }, "AGENT"));
        }

        [Theory]
        [InlineData("CLIENT")]
        [InlineData("TECHNICIAN")]
        public async Task UpdateUserDetailsAsync_ShouldThrowUnauthorized_WhenNonStaffCalls(string role)
        {
            var target = MakeUser(11, Role.CLIENT);
            _userRepo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(target);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateUserDetailsAsync(11, new UpdateUserDetailsDto
                {
                    FirstName = "X",
                    LastName = "Y",
                }, role));
        }

        // ── US-75: Pregled i deaktivacija klijenata ──────────────────────────────

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldSetInactive_WhenAdminDeactivatesClient()
        {
            var client = MakeUser(15, Role.CLIENT);
            _userRepo.Setup(r => r.GetByIdAsync(15)).ReturnsAsync(client);
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _service.ChangeUserStatusAsync(15, isActive: false, currentRole: "ADMINISTRATOR", currentUserId: 1);

            _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.AccountStatus == AccountStatus.INACTIVE)), Times.Once);
        }

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldThrow_WhenAdminDeactivatesOwnAccount()
        {
            var admin = MakeUser(1, Role.ADMINISTRATOR);
            _userRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(admin);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ChangeUserStatusAsync(1, isActive: false, currentRole: "ADMINISTRATOR", currentUserId: 1));
            _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Theory]
        [InlineData(Role.ADMINISTRATOR)]
        [InlineData(Role.AGENT)]
        [InlineData(Role.TECHNICIAN)]
        public async Task ChangeUserStatusAsync_ShouldThrowUnauthorized_WhenAgentDeactivatesNonClientRoles(Role targetRole)
        {
            var target = MakeUser(20, targetRole);
            _userRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(target);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.ChangeUserStatusAsync(20, isActive: false, currentRole: "AGENT", currentUserId: 99));
        }

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldThrowInvalidOperation_WhenDeactivatingAgentWithOpenTickets()
        {
            var agent = MakeUser(30, Role.AGENT);
            _userRepo.Setup(r => r.GetByIdAsync(30)).ReturnsAsync(agent);
            _ticketRepo.Setup(r => r.GetAssignedTicketsForStatsAsync(30))
                .ReturnsAsync(new[]
                {
                    new Ticket { TicketId = 1, Title = "X", CreatorId = 99, Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow, Comments = new List<Comment>(), Assignments = new List<TicketUser>() }
                });

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.ChangeUserStatusAsync(30, isActive: false, currentRole: "ADMINISTRATOR", currentUserId: 1));
        }

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldReactivate_WhenAdminReactivates()
        {
            var inactiveClient = MakeUser(40, Role.CLIENT, AccountStatus.INACTIVE);
            _userRepo.Setup(r => r.GetByIdAsync(40)).ReturnsAsync(inactiveClient);
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _service.ChangeUserStatusAsync(40, isActive: true, currentRole: "ADMINISTRATOR", currentUserId: 1);

            _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.AccountStatus == AccountStatus.ACTIVE)), Times.Once);
        }

        // ── US-89: Upravljanje agentskim nalozima ──────────────────────────────

        [Fact]
        public async Task ChangeUserStatusAsync_ShouldDeactivateAgent_WhenAdminAndNoOpenTickets()
        {
            var agent = MakeUser(50, Role.AGENT);
            _userRepo.Setup(r => r.GetByIdAsync(50)).ReturnsAsync(agent);
            _ticketRepo.Setup(r => r.GetAssignedTicketsForStatsAsync(50))
                .ReturnsAsync(Enumerable.Empty<Ticket>());
            _userRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            await _service.ChangeUserStatusAsync(50, isActive: false, currentRole: "ADMINISTRATOR", currentUserId: 1);

            _userRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.AccountStatus == AccountStatus.INACTIVE && u.Role == Role.AGENT)), Times.Once);
        }

        // ── US-74 / US-89: lista i paginacija ──────────────────────────────────

        [Fact]
        public async Task GetUsersPaginatedAsync_ShouldThrowUnauthorized_WhenClientCalls()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetUsersPaginatedAsync("CLIENT", null, null, null, null, null, 1, 10));
        }

        [Fact]
        public async Task GetUsersPaginatedAsync_ShouldReturnItemsAndPagination_WhenAdminCalls()
        {
            var users = new[] { MakeUser(1, Role.AGENT), MakeUser(2, Role.CLIENT) };
            _userRepo.Setup(r => r.GetUsersPaginatedAsync(null, null, null, null, null, 1, 10))
                .ReturnsAsync((users, 2));

            var result = await _service.GetUsersPaginatedAsync("ADMINISTRATOR", null, null, null, null, null, 1, 10);

            result.Users.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task GetUsersPaginatedAsync_ShouldForwardEnumFilters_WhenProvidedAsStrings()
        {
            _userRepo.Setup(r => r.GetUsersPaginatedAsync(
                    It.Is<Role?>(rl => rl == Role.AGENT),
                    It.Is<AccountStatus?>(s => s == AccountStatus.ACTIVE),
                    It.IsAny<TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus?>(),
                    It.IsAny<string?>(),
                    It.Is<Location?>(l => l == Location.SARAJEVO),
                    1, 10))
                .ReturnsAsync((Enumerable.Empty<User>(), 0));

            var result = await _service.GetUsersPaginatedAsync("ADMINISTRATOR", "AGENT", "ACTIVE", null, null, "SARAJEVO", 1, 10);

            result.Users.Should().BeEmpty();
            _userRepo.VerifyAll();
        }
    }
}
