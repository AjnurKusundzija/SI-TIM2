using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    // PB-70 — MCP Admin Copilot. Prepoznaje intent, poziva read-only MCP alate i Groq modelom
    // (preko GROQ_API_KEY_2) formatira finalni odgovor. NE čita bazu direktno i NE izvršava akcije.
    public class AdminCopilotService : IAdminCopilotService
    {
        private readonly HttpClient _http;
        private readonly IMcpClient _mcp;
        private readonly string? _apiKey2;
        private readonly ILogger<AdminCopilotService> _logger;

        // Intent konstante
        public const string IntentTeamWorkload = "team_workload";
        public const string IntentFaqCoverage = "faq_coverage";
        public const string IntentTicketsNoResponse = "tickets_no_response";
        public const string IntentGeneral = "general_admin_question";
        public const string IntentUnsupported = "unsupported";

        private static readonly Dictionary<string, string> CategoryLabels = new()
        {
            ["INTERNET"] = "Internet",
            ["TV"] = "TV",
            ["MOBILE_NETWORK"] = "Mobilna mreža",
            ["BILLING"] = "Računi",
            ["TECHNICAL_SUPPORT"] = "Tehnička podrška",
        };

        private static readonly Dictionary<string, string> ToolDescriptions = new()
        {
            ["team.workload"] = "Opterećenje timova (otvoreni tiketi, tiketi bez odgovora, workload score)",
            ["ticket.analytics"] = "Agregirani podaci o tiketima i ponavljani problemi",
            ["ticket.search"] = "Pretraga živih tiketa po filterima",
            ["faq.search"] = "Pretraga FAQ baze znanja",
        };

        public AdminCopilotService(HttpClient http, IMcpClient mcp, IConfiguration configuration, ILogger<AdminCopilotService> logger)
        {
            _http = http;
            _mcp = mcp;
            _apiKey2 = configuration["GROQ_API_KEY_2"];
            _logger = logger;
        }

        public async Task<AdminCopilotQueryResponseDto> QueryAsync(AdminCopilotQueryRequestDto request, CancellationToken cancellationToken = default)
        {
            // US-108 / Groq: bez GROQ_API_KEY_2 ne možemo formatirati odgovor — jasna greška.
            if (string.IsNullOrWhiteSpace(_apiKey2) || _apiKey2.StartsWith("YOUR_"))
                throw new InvalidOperationException(
                    "GROQ_API_KEY_2 nije konfigurisan. Dodajte GROQ_API_KEY_2 u .env / docker-compose okruženje za MCP Admin Copilot.");

            var question = (request.Question ?? string.Empty).Trim();
            var intent = DetectIntent(question);

            _logger.LogInformation("AdminCopilot upit primljen. intent={Intent}, pitanje=\"{Question}\"", intent, question);

            // US-108 — nerazumljivo pitanje: tražimo preciziranje (nije greška, vraćamo 200).
            if (intent == IntentUnsupported)
            {
                return new AdminCopilotQueryResponseDto
                {
                    Intent = IntentUnsupported,
                    Answer = "Nisam siguran da sam razumio pitanje. Mogu pomoći s opterećenjem timova, " +
                             "tiketima bez odgovora i pokrivenošću ponavljanih problema FAQ-om.",
                    Message = "Molim precizirajte pitanje (npr. \"Koji tim je najopterećeniji?\" ili " +
                              "\"Koji problemi se ponavljaju, a nisu pokriveni FAQ-om?\").",
                };
            }

            // US-109 — podatke dohvaćamo isključivo preko MCP alata.
            var response = intent switch
            {
                IntentTeamWorkload => await HandleTeamWorkloadAsync(question, cancellationToken),
                IntentFaqCoverage => await HandleFaqCoverageAsync(question, cancellationToken),
                IntentTicketsNoResponse => await HandleTicketsNoResponseAsync(question, cancellationToken),
                _ => await HandleGeneralAsync(question, cancellationToken),
            };

            response.Intent = intent;
            response.Sources = response.UsedTools
                .Select(t => new AdminCopilotSourceDto { Tool = t, Description = ToolDescriptions.GetValueOrDefault(t) })
                .ToList();

            // Groq formatira narativ na osnovu prikupljenih MCP podataka.
            response.Answer = await BuildNarrativeAsync(intent, question, response, cancellationToken);

            _logger.LogInformation("AdminCopilot odgovor spreman. intent={Intent}, alati=[{Tools}]",
                intent, string.Join(", ", response.UsedTools));

            return response;
        }

        // ── Intent detekcija ──────────────────────────────────────────────────
        public static string DetectIntent(string question)
        {
            var q = Normalize(question);
            if (q.Length < 3 || !q.Any(char.IsLetter))
                return IntentUnsupported;

            if (q.Contains("faq") || q.Contains("pokriv"))
                return IntentFaqCoverage;

            if (q.Contains("opterec") || q.Contains("workload") || q.Contains("natrpan") ||
                (q.Contains("tim") && (q.Contains("najvise") || q.Contains("najopt"))))
                return IntentTeamWorkload;

            if (q.Contains("bez odgovora") || q.Contains("neodgovoren") || q.Contains("nije odgovoreno") ||
                q.Contains("stale") || q.Contains("zastarjel") ||
                (q.Contains("odgovor") && (q.Contains("duze") || q.Contains("vise od"))))
                return IntentTicketsNoResponse;

            string[] adminTokens =
            {
                "tiket", "problem", "tim", "analiz", "koliko", "statist", "prikazi", "izvjest",
                "kategorij", "otvoren", "zatvoren", "klijent", "agent", "ocjen", "metrik"
            };
            if (adminTokens.Any(q.Contains))
                return IntentGeneral;

            return IntentUnsupported;
        }

        // ── Handleri intenta ────────────────────────────────────────────────────
        private async Task<AdminCopilotQueryResponseDto> HandleTeamWorkloadAsync(string question, CancellationToken ct)
        {
            var resp = new AdminCopilotQueryResponseDto { UsedTools = { "team.workload", "ticket.analytics" } };

            var workload = await _mcp.CallToolAsync("team.workload", new { }, ct);
            await _mcp.CallToolAsync("ticket.analytics", new { }, ct); // kontekst (US-110: "po potrebi ticket.analytics")

            if (!workload.TryGetProperty("mostLoaded", out var mostLoaded) || mostLoaded.ValueKind == JsonValueKind.Null)
            {
                resp.Message = "Trenutno nema otvorenih tiketa po timovima — podaci su parcijalni.";
                resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Najopterećeniji tim", Value = "Nema podataka" });
                return resp;
            }

            var teamName = GetStr(mostLoaded, "teamName");
            var teamId = GetInt(mostLoaded, "teamId");
            var openTickets = GetInt(mostLoaded, "openTickets");
            var withoutResponse = GetInt(mostLoaded, "ticketsWithoutResponseOver2h");
            var members = GetInt(mostLoaded, "membersCount");
            var avgFirst = GetNullableInt(mostLoaded, "avgFirstResponseMinutes");

            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Najopterećeniji tim", Value = teamName });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Otvoreni tiketi", Value = openTickets.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Bez odgovora > 2h", Value = withoutResponse.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Članova tima", Value = members.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto
            {
                Label = "Prosj. prvi odgovor",
                Value = avgFirst.HasValue ? $"{avgFirst} min" : "nema podataka"
            });

            // Relevantni tiketi najopterećenijeg tima.
            var search = await _mcp.CallToolAsync("ticket.search", new { status = "OPEN", teamId, limit = 5 }, ct);
            resp.UsedTools.Add("ticket.search");
            resp.RelatedTickets = MapRelatedTickets(search);

            resp.Recommendations.Add(new AdminCopilotRecommendationDto
            {
                Title = $"Rasteretiti tim: {teamName}",
                Description = withoutResponse > 0
                    ? $"Tim ima {openTickets} otvorenih tiketa, od čega {withoutResponse} bez odgovora duže od 2h. " +
                      "Razmotrite preraspodjelu ili dodatnu podršku (akciju izvršava administrator ručno)."
                    : $"Tim ima {openTickets} otvorenih tiketa. Pratite opterećenje i razmotrite preraspodjelu ako raste.",
                TeamFilter = teamId.ToString()
            });

            return resp;
        }

        private async Task<AdminCopilotQueryResponseDto> HandleFaqCoverageAsync(string question, CancellationToken ct)
        {
            var resp = new AdminCopilotQueryResponseDto { UsedTools = { "ticket.analytics", "faq.search" } };

            var analytics = await _mcp.CallToolAsync("ticket.analytics", new { }, ct);

            var totalTickets = GetInt(analytics, "totalTickets");
            var openTickets = GetInt(analytics, "openTickets");

            var patterns = ExtractPatterns(analytics);

            if (patterns.Count == 0)
            {
                // US-111 — nema dovoljno konteksta: predloži ručnu analizu i prikaži relevantne tikete.
                var openSearch = await _mcp.CallToolAsync("ticket.search", new { status = "OPEN", limit = 10 }, ct);
                resp.UsedTools.Add("ticket.search");
                resp.RelatedTickets = MapRelatedTickets(openSearch);
                resp.Message = "Nema dovoljno konteksta za automatsku FAQ analizu — preporučuje se ručna analiza priloženih tiketa.";
                resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Ukupno tiketa", Value = totalTickets.ToString() });
                return resp;
            }

            var query = string.Join(" ", patterns.Select(p => p.Pattern));
            var faqRes = await _mcp.CallToolAsync("faq.search", new { query, limit = 30 }, ct);
            var faqs = MapFaqResults(faqRes);

            var uncovered = 0;
            foreach (var p in patterns)
            {
                var match = faqs.FirstOrDefault(f =>
                    f.Question.ToLowerInvariant().Contains(p.Pattern) ||
                    f.Answer.ToLowerInvariant().Contains(p.Pattern));
                var covered = match != null;
                if (!covered) uncovered++;

                var label = CategoryLabels.GetValueOrDefault(p.Category, p.Category);
                resp.FaqCoverage.Add(new AdminCopilotFaqCoverageDto
                {
                    Problem = p.Pattern,
                    OccurrenceCount = p.Count,
                    Covered = covered,
                    MatchedFaqQuestion = match?.Question,
                    SuggestedQuestion = covered ? null : $"Kako riješiti problem: \"{p.Pattern}\" ({label})?",
                    SuggestedAnswer = covered
                        ? null
                        : $"Nacrt: koraci za rješavanje problema \"{p.Pattern}\" iz kategorije {label}. " +
                          "Dopuniti prema internim uputstvima (FAQ se NE kreira automatski)."
                });
            }

            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Ukupno tiketa", Value = totalTickets.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Otvoreni tiketi", Value = openTickets.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Ponavljani problemi", Value = patterns.Count.ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Nepokriveno FAQ-om", Value = uncovered.ToString() });

            foreach (var fc in resp.FaqCoverage.Where(f => !f.Covered).Take(3))
            {
                resp.Recommendations.Add(new AdminCopilotRecommendationDto
                {
                    Title = $"Predloži FAQ: \"{fc.Problem}\"",
                    Description = fc.SuggestedQuestion ?? $"Razmotrite dodavanje FAQ stavke za problem \"{fc.Problem}\"."
                });
            }

            if (uncovered == 0)
                resp.Message = "Svi prepoznati ponavljani problemi su pokriveni postojećim FAQ-om.";

            return resp;
        }

        private async Task<AdminCopilotQueryResponseDto> HandleTicketsNoResponseAsync(string question, CancellationToken ct)
        {
            var resp = new AdminCopilotQueryResponseDto { UsedTools = { "ticket.search", "ticket.analytics" } };

            var minutes = ParseThresholdMinutes(question) ?? 120;
            var search = await _mcp.CallToolAsync("ticket.search", new { status = "OPEN", olderThanMinutes = minutes, limit = 20 }, ct);
            var analytics = await _mcp.CallToolAsync("ticket.analytics", new { }, ct);

            resp.RelatedTickets = MapRelatedTickets(search);
            var count = resp.RelatedTickets.Count;

            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Tiketi bez odgovora", Value = count.ToString(), Hint = $"prag {minutes} min" });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Ukupno otvorenih", Value = GetInt(analytics, "openTickets").ToString() });

            if (count > 0)
            {
                resp.Recommendations.Add(new AdminCopilotRecommendationDto
                {
                    Title = "Hitno odgovoriti na tikete",
                    Description = $"{count} otvorenih tiketa nema odgovor duže od {minutes} minuta. Prioritizirajte odgovore."
                });
            }
            else
            {
                resp.Message = $"Nema otvorenih tiketa bez odgovora dužeg od {minutes} minuta.";
            }

            return resp;
        }

        private async Task<AdminCopilotQueryResponseDto> HandleGeneralAsync(string question, CancellationToken ct)
        {
            var resp = new AdminCopilotQueryResponseDto { UsedTools = { "ticket.analytics" } };
            var analytics = await _mcp.CallToolAsync("ticket.analytics", new { }, ct);

            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Ukupno tiketa", Value = GetInt(analytics, "totalTickets").ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Otvoreni", Value = GetInt(analytics, "openTickets").ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Zatvoreni", Value = GetInt(analytics, "closedTickets").ToString() });
            resp.Metrics.Add(new AdminCopilotMetricDto { Label = "Zastarjeli", Value = GetInt(analytics, "staleTickets").ToString() });

            var patterns = ExtractPatterns(analytics);
            if (patterns.Count > 0)
            {
                var top = patterns[0];
                resp.Metrics.Add(new AdminCopilotMetricDto
                {
                    Label = "Najčešći problem",
                    Value = top.Pattern,
                    Hint = $"{top.Count} tiketa"
                });
            }

            var stale = GetInt(analytics, "staleTickets");
            if (stale > 0)
            {
                resp.Recommendations.Add(new AdminCopilotRecommendationDto
                {
                    Title = "Riješiti zastarjele tikete",
                    Description = $"Postoji {stale} zastarjelih otvorenih tiketa. Razmotrite njihovo prioritiziranje."
                });
            }

            return resp;
        }

        // ── Groq narativ ──────────────────────────────────────────────────────
        private async Task<string> BuildNarrativeAsync(string intent, string question, AdminCopilotQueryResponseDto data, CancellationToken ct)
        {
            var context = JsonSerializer.Serialize(new
            {
                intent,
                metrics = data.Metrics,
                recommendations = data.Recommendations,
                relatedTickets = data.RelatedTickets,
                faqCoverage = data.FaqCoverage,
                usedTools = data.UsedTools,
                note = data.Message
            });

            var prompt = $$"""
                Ti si asistent administratoru telekom helpdesk sistema (MCP Admin Copilot).
                Odgovori ISKLJUČIVO na osnovu sljedećih podataka koji su dobijeni preko MCP alata (živi podaci).
                NE izmišljaj podatke. Ako podaci nisu dovoljni, jasno to reci.

                PITANJE ADMINISTRATORA: {{question}}

                PODACI (JSON, iz MCP alata):
                {{context}}

                Napiši odgovor na bosanskom jeziku, jasno strukturiran u sekcijama:
                - Sažetak (2-4 rečenice)
                - Ključne metrike (kratko, na osnovu polja "metrics")
                - Preporuke (na osnovu polja "recommendations"; naglasi da se akcije ne izvršavaju automatski)
                - Korišteni izvori (nabroji "usedTools")

                Budi koncizan i konkretan. Ako je "note" prisutan, uvaži ga u odgovoru.
                """;

            try
            {
                var text = await CallGroqAsync(prompt, temperature: 0.3, maxTokens: 1200, ct);
                return string.IsNullOrWhiteSpace(text) ? BuildFallbackNarrative(data) : text.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Groq formatiranje nije uspjelo — koristi se deterministički sažetak.");
                return BuildFallbackNarrative(data);
            }
        }

        private static string BuildFallbackNarrative(AdminCopilotQueryResponseDto data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sažetak na osnovu živih podataka (MCP alati):");
            foreach (var m in data.Metrics)
                sb.AppendLine($"- {m.Label}: {m.Value}{(string.IsNullOrEmpty(m.Hint) ? "" : $" ({m.Hint})")}");
            if (data.Recommendations.Count > 0)
            {
                sb.AppendLine("Preporuke:");
                foreach (var r in data.Recommendations)
                    sb.AppendLine($"- {r.Title}: {r.Description}");
            }
            if (!string.IsNullOrEmpty(data.Message))
                sb.AppendLine(data.Message);
            sb.AppendLine($"Korišteni izvori: {string.Join(", ", data.UsedTools)}");
            return sb.ToString().Trim();
        }

        private async Task<string> CallGroqAsync(string prompt, double temperature, int maxTokens, CancellationToken ct)
        {
            const string url = "https://api.groq.com/openai/v1/chat/completions";

            var body = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[] { new { role = "user", content = prompt } },
                temperature,
                max_tokens = maxTokens
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(body)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey2);

            using var response = await _http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            using var doc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct)
                ?? throw new InvalidOperationException("Prazan odgovor Groq API-ja.");

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;
        }

        // ── Pomoćne (parsing/mapping) ───────────────────────────────────────────
        private sealed record ProblemPattern(string Pattern, int Count, string Category);

        private static List<ProblemPattern> ExtractPatterns(JsonElement analytics)
        {
            var list = new List<ProblemPattern>();
            if (analytics.TryGetProperty("topProblemPatterns", out var pats) && pats.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in pats.EnumerateArray())
                    list.Add(new ProblemPattern(
                        GetStr(p, "pattern").ToLowerInvariant(),
                        GetInt(p, "count"),
                        GetStr(p, "category")));
            }

            // Fallback: iskoristi top kategorije ako nema jasnih obrazaca.
            if (list.Count == 0 && analytics.TryGetProperty("topCategories", out var cats) && cats.ValueKind == JsonValueKind.Array)
            {
                foreach (var c in cats.EnumerateArray().Take(5))
                {
                    var cat = GetStr(c, "category");
                    list.Add(new ProblemPattern(cat.ToLowerInvariant(), GetInt(c, "count"), cat));
                }
            }

            return list.Take(6).ToList();
        }

        private sealed record FaqResult(int FaqId, string Question, string Answer, string? Category, double RelevanceScore);

        private static List<FaqResult> MapFaqResults(JsonElement faqRes)
        {
            var list = new List<FaqResult>();
            if (faqRes.TryGetProperty("results", out var results) && results.ValueKind == JsonValueKind.Array)
            {
                foreach (var r in results.EnumerateArray())
                    list.Add(new FaqResult(
                        GetInt(r, "faqId"),
                        GetStr(r, "question"),
                        GetStr(r, "answer"),
                        r.TryGetProperty("category", out var c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null,
                        r.TryGetProperty("relevanceScore", out var s) && s.ValueKind == JsonValueKind.Number ? s.GetDouble() : 0));
            }
            return list;
        }

        private static List<AdminCopilotRelatedTicketDto> MapRelatedTickets(JsonElement search)
        {
            var list = new List<AdminCopilotRelatedTicketDto>();
            if (search.TryGetProperty("tickets", out var tickets) && tickets.ValueKind == JsonValueKind.Array)
            {
                foreach (var t in tickets.EnumerateArray())
                    list.Add(new AdminCopilotRelatedTicketDto
                    {
                        TicketId = GetInt(t, "ticketId"),
                        Title = GetStr(t, "title"),
                        Status = GetStr(t, "status"),
                        Priority = GetStr(t, "priority"),
                        TeamName = t.TryGetProperty("teamName", out var tn) && tn.ValueKind == JsonValueKind.String ? tn.GetString() : null,
                        MinutesWithoutResponse = GetNullableInt(t, "minutesWithoutResponse"),
                    });
            }
            return list;
        }

        public static int? ParseThresholdMinutes(string question)
        {
            var q = Normalize(question);
            var match = System.Text.RegularExpressions.Regex.Match(q, @"(\d+)\s*(sat|sata|sati|h|min|minut)");
            if (!match.Success) return null;
            var n = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var unit = match.Groups[2].Value;
            return unit.StartsWith("min") ? n : n * 60;
        }

        private static int GetInt(JsonElement e, string prop) =>
            e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : 0;

        private static int? GetNullableInt(JsonElement e, string prop) =>
            e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number ? v.GetInt32() : null;

        private static string GetStr(JsonElement e, string prop) =>
            e.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String ? (v.GetString() ?? string.Empty) : string.Empty;

        // Normalizacija: lowercase + uklanjanje dijakritika (č,ć,ž,š,đ -> c,c,z,s,d).
        public static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var lowered = input.ToLowerInvariant().Replace("đ", "d");
            var decomposed = lowered.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(decomposed.Length);
            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
