using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Auth;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Integration
{
    // PB-19: Integracijsko testiranje toka prijave kroz Controller → Service → Repository
    public class AuthIntegrationTests
    {
        private const string TestPassword = "Password123!";

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = "integration-test-secret-key-must-be-at-least-32-chars!!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                })
                .Build();

        private static AuthController CreateController(ApplicationDbContext context)
        {
            var userRepo = new UserRepository(context);
            var tokenRepo = new RefreshTokenRepository(context);
            var service = new AuthService(userRepo, tokenRepo, BuildConfig());
            return new AuthController(service);
        }

        private static User MakeActiveUser(int id, string email, Role role = Role.CLIENT) => new()
        {
            UserId = id,
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Username = $"user{id}",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword),
            AccountStatus = AccountStatus.ACTIVE,
            Role = role,
        };

        // US-1: ispravni kredencijali vraćaju token kroz sve slojeve
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsJwtTokenThroughFullStack()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeActiveUser(1, "klijent@test.ba"));
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = await controller.Login(new LoginRequestDto
            {
                Email = "klijent@test.ba",
                Password = TestPassword,
            });

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeAssignableTo<LoginResponseDto>().Subject;
            body.AccessToken.Should().NotBeNullOrEmpty();
            body.RefreshToken.Should().NotBeNullOrEmpty();
            body.Email.Should().Be("klijent@test.ba");
        }

        // US-3: pogrešna lozinka vraća 401 s generičkom porukom (ne otkriva koji podatak je pogrešan)
        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorizedWithGenericMessage()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeActiveUser(2, "agent@test.ba"));
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = await controller.Login(new LoginRequestDto
            {
                Email = "agent@test.ba",
                Password = "WrongPassword!",
            });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // US-3: nepostojeći email vraća 401 (isti odgovor kao pogrešna lozinka — ne otkriva koji podatak je pogrešan)
        [Fact]
        public async Task Login_WithUnknownEmail_ReturnsUnauthorized()
        {
            using var context = CreateDbContext();

            var controller = CreateController(context);
            var result = await controller.Login(new LoginRequestDto
            {
                Email = "nepostoji@test.ba",
                Password = TestPassword,
            });

            result.Should().BeOfType<UnauthorizedObjectResult>();
        }

        // US-2: odjava pohranjuje revokovan refresh token u bazi
        [Fact]
        public async Task Logout_AfterSuccessfulLogin_RevokesRefreshTokenInDatabase()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeActiveUser(3, "logout@test.ba"));
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var loginResult = await controller.Login(new LoginRequestDto
            {
                Email = "logout@test.ba",
                Password = TestPassword,
            });
            var loginBody = ((OkObjectResult)loginResult).Value as LoginResponseDto;
            loginBody.Should().NotBeNull();

            var logoutResult = await controller.Logout(new RefreshRequestDto
            {
                RefreshToken = loginBody!.RefreshToken,
            });

            logoutResult.Should().BeOfType<NoContentResult>();
            context.RefreshTokens.Should().ContainSingle(rt => rt.IsRevoked);
        }

        // US-1: refresh token se može koristiti za novi access token
        [Fact]
        public async Task Refresh_WithValidToken_ReturnsNewAccessToken()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeActiveUser(4, "refresh@test.ba"));
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var loginResult = await controller.Login(new LoginRequestDto
            {
                Email = "refresh@test.ba",
                Password = TestPassword,
            });
            var loginBody = ((OkObjectResult)loginResult).Value as LoginResponseDto;

            var refreshResult = await controller.Refresh(new RefreshRequestDto
            {
                RefreshToken = loginBody!.RefreshToken,
            });

            var ok = refreshResult.Should().BeOfType<OkObjectResult>().Subject;
            var body = ok.Value.Should().BeAssignableTo<RefreshResponseDto>().Subject;
            body.AccessToken.Should().NotBeNullOrEmpty();
        }
    }
}
