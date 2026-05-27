using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.AI
{
    // PB-70 / US-108..US-111 — AdminCopilotService orkestracija (intent -> MCP -> Groq).
    public class AdminCopilotServiceTests
    {
        private readonly Mock<IMcpClient> _mcp = new();

        public AdminCopilotServiceTests()
        {
            // Default: svaki alat vraća prazan objekat. Test-specifični setupi (registrovani
            // kasnije) imaju prioritet u Moq-u.
            _mcp.Setup(m => m.CallToolAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("{}"));
        }

        // ── Fake Groq HTTP handler ────────────────────────────────────────────
        private sealed class FakeGroqHandler : HttpMessageHandler
        {
            public int Calls { get; private set; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Calls++;
                var json = "{\"choices\":[{\"message\":{\"content\":\"Sažetak na bosanskom jeziku.\"}}]}";
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
            }
        }

        private static JsonElement Json(string json) => JsonDocument.Parse(json).RootElement.Clone();

        private AdminCopilotService BuildService(string? groqKey = "test-key-2", FakeGroqHandler? handler = null)
        {
            handler ??= new FakeGroqHandler();
            var http = new HttpClient(handler);
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(groqKey is null
                    ? new Dictionary<string, string?>()
                    : new Dictionary<string, string?> { ["GROQ_API_KEY_2"] = groqKey })
                .Build();

            return new AdminCopilotService(http, _mcp.Object, config, NullLogger<AdminCopilotService>.Instance);
        }

        private void SetupWorkload()
        {
            _mcp.Setup(m => m.CallToolAsync("team.workload", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("""
                {
                  "teams": [{"teamId":1,"teamName":"Internet Tim","openTickets":5,"membersCount":3,"ticketsWithoutResponseOver2h":2,"avgFirstResponseMinutes":45,"workloadScore":9}],
                  "mostLoaded": {"teamId":1,"teamName":"Internet Tim","openTickets":5,"membersCount":3,"ticketsWithoutResponseOver2h":2,"avgFirstResponseMinutes":45,"workloadScore":9},
                  "criterion": "workloadScore"
                }
                """));
            _mcp.Setup(m => m.CallToolAsync("ticket.analytics", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("""{"totalTickets":13,"openTickets":9,"closedTickets":4,"staleTickets":1,"topCategories":[{"category":"INTERNET","count":4}],"topProblemPatterns":[]}"""));
            _mcp.Setup(m => m.CallToolAsync("ticket.search", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("""{"count":1,"tickets":[{"ticketId":1,"title":"Internet ne radi","status":"OPEN","priority":"HIGH","teamName":"Internet Tim","minutesWithoutResponse":200}]}"""));
        }

        // ── US-110 ──────────────────────────────────────────────────────────────
        [Fact]
        public async Task TeamWorkloadQuestion_CallsTeamWorkloadTool()
        {
            SetupWorkload();
            var service = BuildService();

            var result = await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "Koji tim je najopterećeniji?" });

            result.Intent.Should().Be(AdminCopilotService.IntentTeamWorkload);
            _mcp.Verify(m => m.CallToolAsync("team.workload", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
            result.RelatedTickets.Should().NotBeEmpty();
            result.Metrics.Should().Contain(m => m.Label == "Najopterećeniji tim" && m.Value == "Internet Tim");
        }

        // ── US-111 ──────────────────────────────────────────────────────────────
        [Fact]
        public async Task FaqCoverageQuestion_CallsAnalyticsAndFaqSearch()
        {
            _mcp.Setup(m => m.CallToolAsync("ticket.analytics", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("""{"totalTickets":10,"openTickets":6,"topCategories":[{"category":"INTERNET","count":4}],"topProblemPatterns":[{"pattern":"internet","count":4,"category":"INTERNET","sampleTicketIds":[1,2]}]}"""));
            _mcp.Setup(m => m.CallToolAsync("faq.search", It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Json("""{"count":1,"results":[{"faqId":2,"question":"Internet je spor","answer":"Restartujte ruter","category":"Internet","relevanceScore":1.0}]}"""));
            var service = BuildService();

            var result = await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?" });

            result.Intent.Should().Be(AdminCopilotService.IntentFaqCoverage);
            _mcp.Verify(m => m.CallToolAsync("ticket.analytics", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _mcp.Verify(m => m.CallToolAsync("faq.search", It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
            result.FaqCoverage.Should().NotBeEmpty();
        }

        // ── US-109 — MCP nedostupan ──────────────────────────────────────────────
        [Fact]
        public async Task McpUnavailable_ThrowsMcpUnavailableException()
        {
            _mcp.Setup(m => m.CallToolAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new McpUnavailableException("MCP server trenutno nije dostupan."));
            var service = BuildService();

            var act = async () => await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "Koji tim je najopterećeniji?" });

            await act.Should().ThrowAsync<McpUnavailableException>();
        }

        // ── Groq ključ nije konfigurisan ──────────────────────────────────────────
        [Fact]
        public async Task MissingGroqKey_ThrowsWithKeyName()
        {
            var service = BuildService(groqKey: null);

            var act = async () => await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "Koji tim je najopterećeniji?" });

            (await act.Should().ThrowAsync<InvalidOperationException>())
                .Which.Message.Should().Contain("GROQ_API_KEY_2");
        }

        // ── Odgovor nije prazan i ima sources/usedTools ──────────────────────────
        [Fact]
        public async Task SupportedQuestion_ReturnsNonEmptyAnswerWithSourcesAndTools()
        {
            SetupWorkload();
            var service = BuildService();

            var result = await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "Koji tim je najopterećeniji?" });

            result.Answer.Should().NotBeNullOrWhiteSpace();
            result.UsedTools.Should().NotBeEmpty();
            result.Sources.Should().NotBeEmpty();
            result.UsedTools.Should().Contain("team.workload");
        }

        // ── US-108 — nerazumljivo pitanje ──────────────────────────────────────────
        [Fact]
        public async Task UnsupportedQuestion_ReturnsClarificationMessage()
        {
            var service = BuildService();

            var result = await service.QueryAsync(new AdminCopilotQueryRequestDto { Question = "blabla nešto" });

            result.Intent.Should().Be(AdminCopilotService.IntentUnsupported);
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Answer.Should().NotBeNullOrWhiteSpace();
            _mcp.Verify(m => m.CallToolAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        // ── Intent detekcija (jedinično) ──────────────────────────────────────────
        [Theory]
        [InlineData("Koji tim je najopterećeniji?", "team_workload")]
        [InlineData("Prikaži tikete bez odgovora duže od 2 sata", "tickets_no_response")]
        [InlineData("Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?", "faq_coverage")]
        [InlineData("Koliko ukupno ima tiketa?", "general_admin_question")]
        [InlineData("xx", "unsupported")]
        public void DetectIntent_MapsCorrectly(string question, string expected)
        {
            AdminCopilotService.DetectIntent(question).Should().Be(expected);
        }

        [Fact]
        public void ParseThresholdMinutes_ParsesHoursAndMinutes()
        {
            AdminCopilotService.ParseThresholdMinutes("bez odgovora duže od 2 sata").Should().Be(120);
            AdminCopilotService.ParseThresholdMinutes("duže od 30 minuta").Should().Be(30);
            AdminCopilotService.ParseThresholdMinutes("bez brojeva").Should().BeNull();
        }
    }
}
