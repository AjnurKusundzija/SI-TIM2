using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using TelecomSupportSystem.BLL.DTOs.Auth;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.Tests.Auth;

/// <summary>
/// Unit tests for AuthService covering PB-19 (US-1, US-2, US-3).
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRefreshTokenRepository> _tokenRepo = new();
    private readonly AuthService _sut;

    private const string TestPassword = "Password123!";
    private static readonly string TestPasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword);

    public AuthServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT_KEY"] = "unit-test-secret-key-must-be-at-least-32-chars!!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
            })
            .Build();

        _sut = new AuthService(_userRepo.Object, _tokenRepo.Object, config);

        // Default setup so LoginAsync can persist a refresh token
        _tokenRepo.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.RevokeAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
        _tokenRepo.Setup(r => r.RevokeAllForUserAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static User MakeUser(
        Role role = Role.CLIENT,
        AccountStatus status = AccountStatus.ACTIVE,
        string email = "user@example.com") => new()
    {
        UserId = 42,
        FirstName = "Emir",
        LastName = "Hadzic",
        Email = email,
        PasswordHash = TestPasswordHash,
        Role = role,
        AccountStatus = status,
    };

    private static RefreshToken MakeRefreshToken(
        User user,
        bool isRevoked = false,
        int daysUntilExpiry = 7) => new()
    {
        RefreshTokenId = 1,
        Token = "stored-hash",
        UserId = user.UserId,
        User = user,
        IsRevoked = isRevoked,
        CreatedAt = DateTime.UtcNow,
        ExpiresAt = DateTime.UtcNow.AddDays(daysUntilExpiry),
    };

    // ─── LoginAsync ───────────────────────────────────────────────────────────

    // US-3: sistema ne smije otkriti da li je email ili lozinka pogrešna
    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsNull()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = "nobody@example.com", Password = "any" });

        Assert.Null(result);
    }

    // US-3: isti odgovor (null) kad je lozinka pogrešna — nema razlike
    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var user = MakeUser();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("DifferentPass!");
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        Assert.Null(result);
    }

    // US-1: neaktivni nalog ne smije dobiti pristup
    [Fact]
    public async Task LoginAsync_InactiveAccount_ReturnsNull()
    {
        var user = MakeUser(status: AccountStatus.INACTIVE);
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        Assert.Null(result);
    }

    // US-1: uspješna prijava vraća odgovor s tokenima i podacima korisnika
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsLoginResponseDto()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        Assert.NotNull(result);
        Assert.Equal(user.UserId, result.UserId);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.Email, result.Email);
    }

    // US-1: JWT access token i refresh token moraju biti neprazni
    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsBothNonEmptyTokens()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
    }

    // US-1: refresh token se sprema u bazu pri uspješnoj prijavi
    [Fact]
    public async Task LoginAsync_ValidCredentials_PersistsRefreshToken()
    {
        var user = MakeUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        _tokenRepo.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => t.UserId == user.UserId)), Times.Once);
    }

    // US-1/US-2: uloga se ispravno uključuje u odgovor za sve četiri uloge
    [Theory]
    [InlineData(Role.CLIENT)]
    [InlineData(Role.AGENT)]
    [InlineData(Role.TECHNICIAN)]
    [InlineData(Role.ADMINISTRATOR)]
    public async Task LoginAsync_ActiveAccount_ReturnsCorrectRoleString(Role role)
    {
        var user = MakeUser(role: role);
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var result = await _sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword });

        Assert.NotNull(result);
        Assert.Equal(role.ToString(), result.Role);
    }

    // Konfiguracija: JWT_KEY nije postavljen — GenerateAccessToken mora baciti iznimku
    [Fact]
    public async Task LoginAsync_MissingJwtKey_ThrowsInvalidOperationException()
    {
        var configWithoutKey = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
            })
            .Build();

        var sut = new AuthService(_userRepo.Object, _tokenRepo.Object, configWithoutKey);

        var user = MakeUser();
        _userRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.LoginAsync(new LoginRequestDto { Email = user.Email, Password = TestPassword }));
    }

    // ─── RefreshAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task RefreshAsync_TokenNotFound_ReturnsNull()
    {
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken?)null);

        var result = await _sut.RefreshAsync("nonexistent-token");

        Assert.Null(result);
    }

    // Sigurnost: prezentacija opozvanog tokena opozvava cijelu sesiju
    [Fact]
    public async Task RefreshAsync_RevokedToken_RevokesEntireSessionAndReturnsNull()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user, isRevoked: true);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RefreshAsync("revoked-token");

        Assert.Null(result);
        _tokenRepo.Verify(r => r.RevokeAllForUserAsync(user.UserId), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_ExpiredToken_ReturnsNull()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user, daysUntilExpiry: -1);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RefreshAsync("expired-token");

        Assert.Null(result);
    }

    // US-2: neaktivni nalog ne može dobiti novi token
    [Fact]
    public async Task RefreshAsync_InactiveUser_ReturnsNull()
    {
        var user = MakeUser(status: AccountStatus.INACTIVE);
        var stored = MakeRefreshToken(user);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RefreshAsync("valid-format-token");

        Assert.Null(result);
    }

    // US-2: valjani refresh token vraća novi par tokena (rotacija)
    [Fact]
    public async Task RefreshAsync_ValidToken_ReturnsNewTokenPair()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RefreshAsync("valid-token");

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
    }

    // Sigurnost: stari token mora biti opozvan pri rotaciji
    [Fact]
    public async Task RefreshAsync_ValidToken_RevokesOldToken()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        await _sut.RefreshAsync("valid-token");

        _tokenRepo.Verify(r => r.RevokeAsync(stored), Times.Once);
    }

    // ─── RevokeAsync (Logout) ─────────────────────────────────────────────────

    // US-2: odjava opozvava refresh token
    [Fact]
    public async Task RevokeAsync_ValidToken_ReturnsTrueAndRevokesToken()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RevokeAsync("valid-token");

        Assert.True(result);
        _tokenRepo.Verify(r => r.RevokeAsync(stored), Times.Once);
    }

    // US-2: već opozvan token vraća false — nema duplikata odjave
    [Fact]
    public async Task RevokeAsync_AlreadyRevokedToken_ReturnsFalse()
    {
        var user = MakeUser();
        var stored = MakeRefreshToken(user, isRevoked: true);
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync(stored);

        var result = await _sut.RevokeAsync("revoked-token");

        Assert.False(result);
        _tokenRepo.Verify(r => r.RevokeAsync(It.IsAny<RefreshToken>()), Times.Never);
    }

    // US-2: nepostojeći token vraća false bez greške
    [Fact]
    public async Task RevokeAsync_TokenNotFound_ReturnsFalse()
    {
        _tokenRepo.Setup(r => r.GetByTokenAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken?)null);

        var result = await _sut.RevokeAsync("missing-token");

        Assert.False(result);
    }
}
