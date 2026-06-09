using System.Diagnostics;
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

namespace TelecomSupportSystem.Tests.Performance
{
    // PB-19: Performansno testiranje toka prijave
    public class AuthPerformanceTests
    {
        private const int MaxLoginTimeMilliseconds = 5000;
        private const string TestPassword = "Password123!";

        private static IConfiguration BuildConfig() =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT_KEY"] = "performance-test-secret-key-must-be-at-least-32-chars!!",
                    ["Jwt:Issuer"] = "TestIssuer",
                    ["Jwt:Audience"] = "TestAudience",
                })
                .Build();

        // US-1: tok prijave (Controller → Service → Repository) se izvrsava u prihvatljivom vremenu
        [Fact]
        public async Task Login_ShouldCompleteWithinTimeLimit_InTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Users.Add(new User
            {
                UserId = 1,
                FirstName = "Perf",
                LastName = "Test",
                Email = "perf@test.ba",
                Username = "perftest",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(TestPassword),
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.CLIENT,
            });
            await context.SaveChangesAsync();

            var controller = new AuthController(
                new AuthService(new UserRepository(context), new RefreshTokenRepository(context), BuildConfig()));

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.Login(new LoginRequestDto
            {
                Email = "perf@test.ba",
                Password = TestPassword,
            });
            stopwatch.Stop();

            result.Should().BeOfType<OkObjectResult>();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxLoginTimeMilliseconds,
                because: $"login mora biti brz — mjereno {stopwatch.ElapsedMilliseconds}ms, prag {MaxLoginTimeMilliseconds}ms");
        }
    }
}
