using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelecomSupportSystem.BLL.DTOs.AuditLogs;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using Xunit;

namespace TelecomSupportSystem.Tests.AuditLog;

public class AuditLogControllerIntegrationTests
{
    [Fact]
    public async Task GetAuditLogs_ReturnsForbidden_ForAgent()
    {
        using var factory = CreateFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", "2");
        client.DefaultRequestHeaders.Add("X-Test-Role", "AGENT");

        var response = await client.GetAsync("/api/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsUnauthorized_WithoutToken()
    {
        using var factory = CreateFactory();
        var response = await factory.CreateClient().GetAsync("/api/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsPagedResults_ForAdministrator()
    {
        using var factory = CreateFactory();
        var client = CreateAdminClient(factory);

        var result = await client.GetFromJsonAsync<AuditLogResponseDto>("/api/audit-logs?page=1&pageSize=10");

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(25);
        result.Items.Should().HaveCount(10);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetAuditLogs_FiltersByActionType()
    {
        using var factory = CreateFactory();
        var client = CreateAdminClient(factory);

        var result = await client.GetFromJsonAsync<AuditLogResponseDto>("/api/audit-logs?actionType=TICKET_CREATED&pageSize=25");

        result.Should().NotBeNull();
        result!.Items.Should().OnlyContain(item => item.ActionType == "TICKET_CREATED");
    }

    [Fact]
    public async Task GetAuditLogDetail_ReturnsNotFound_ForMissingId()
    {
        using var factory = CreateFactory();
        var client = CreateAdminClient(factory);

        var response = await client.GetAsync("/api/audit-logs/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAuditLogDetail_ReturnsDeserializedOldAndNewValues()
    {
        using var factory = CreateFactory();
        var client = CreateAdminClient(factory);

        var json = await client.GetStringAsync("/api/audit-logs/2");
        using var document = JsonDocument.Parse(json);

        document.RootElement.GetProperty("oldValue").ValueKind.Should().Be(JsonValueKind.Object);
        document.RootElement.GetProperty("newValue").ValueKind.Should().Be(JsonValueKind.Object);
        document.RootElement.GetProperty("oldValue").GetProperty("status").GetString().Should().Be("OPEN");
        document.RootElement.GetProperty("newValue").GetProperty("status").GetString().Should().Be("CLOSED");
    }

    private static HttpClient CreateAdminClient(WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-UserId", "1");
        client.DefaultRequestHeaders.Add("X-Test-Role", "ADMINISTRATOR");
        client.DefaultRequestHeaders.Add("X-Test-Email", "admin@example.com");
        return client;
    }

    private static WebApplicationFactory<Program> CreateFactory()
    {
        Environment.SetEnvironmentVariable("JWT_KEY", "test-jwt-key-for-audit-log-integration-tests-12345");
        var databaseName = Guid.NewGuid().ToString();
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureServices(services =>
                {
                    foreach (var descriptor in services
                        .Where(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                                    d.ServiceType == typeof(DbContextOptions) ||
                                    d.ServiceType.Name.Contains("DbContextOptions") ||
                                    d.ServiceType.Name.Contains("IDbContextOptionsConfiguration"))
                        .ToList())
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(databaseName));

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                        options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                        options.DefaultForbidScheme = TestAuthHandler.SchemeName;
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

                });
            });

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();
        Seed(db);

        return factory;
    }

    private static void Seed(ApplicationDbContext db)
    {
        var admin = new User
        {
            UserId = 1,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            Username = "admin",
            PasswordHash = "hash",
            Role = Role.ADMINISTRATOR,
            AccountStatus = AccountStatus.ACTIVE,
            Location = Location.SARAJEVO
        };
        var agent = new User
        {
            UserId = 2,
            FirstName = "Ajdin",
            LastName = "Hodzic",
            Email = "ajdin@example.com",
            Username = "agent",
            PasswordHash = "hash",
            Role = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            Location = Location.SARAJEVO
        };

        db.Users.AddRange(admin, agent);
        for (var i = 1; i <= 25; i++)
        {
            db.AuditLogs.Add(new DAL.Entities.AuditLog
            {
                Id = i,
                Timestamp = DateTime.UtcNow.AddMinutes(-i),
                UserId = i % 2 == 0 ? agent.UserId : admin.UserId,
                ActionType = i % 3 == 0 ? "TICKET_CREATED" : i % 3 == 1 ? "USER_LOGIN" : "TICKET_STATUS_CHANGED",
                EntityType = i % 2 == 0 ? "Ticket" : "User",
                EntityId = i.ToString(),
                Description = $"Test action {i}",
                OldValue = i == 2 ? "{\"status\":\"OPEN\"}" : null,
                NewValue = i == 2 ? "{\"status\":\"CLOSED\"}" : null,
                IpAddress = "192.168.1.1"
            });
        }

        db.SaveChanges();
    }

    private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Test-UserId", out var userId) ||
                !Request.Headers.TryGetValue("X-Test-Role", out var role))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role.ToString()),
                new(ClaimTypes.Email, Request.Headers.TryGetValue("X-Test-Email", out var email) ? email.ToString() : "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
