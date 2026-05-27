using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.BLL.Services.Mcp
{
    // PB-70 / US-109 — minimalan MCP klijent preko Streamable HTTP transporta (JSON-RPC 2.0).
    // Tok po pozivu: initialize -> notifications/initialized -> tools/call.
    // MCP server je konfigurisan sa enableJsonResponse, pa su odgovori application/json.
    public class McpClient : IMcpClient
    {
        private readonly HttpClient _http;
        private readonly string _serverUrl;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public McpClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _serverUrl = configuration["MCP_SERVER_URL"] ?? "http://localhost:3001/mcp";
        }

        public async Task<JsonElement> CallToolAsync(string toolName, object arguments, CancellationToken cancellationToken = default)
        {
            try
            {
                // 1) initialize — uspostavi sesiju i preuzmi Mcp-Session-Id
                var initBody = new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    method = "initialize",
                    @params = new
                    {
                        protocolVersion = "2024-11-05",
                        capabilities = new { },
                        clientInfo = new { name = "telecom-backend", version = "1.0.0" }
                    }
                };

                using var initResponse = await PostAsync(initBody, sessionId: null, cancellationToken);
                if (!initResponse.IsSuccessStatusCode)
                    throw new McpUnavailableException($"MCP initialize neuspješan (HTTP {(int)initResponse.StatusCode}).");

                var sessionId = initResponse.Headers.TryGetValues("Mcp-Session-Id", out var values)
                    ? values.FirstOrDefault()
                    : null;

                // 2) notifications/initialized (notifikacija, bez id)
                var initializedBody = new { jsonrpc = "2.0", method = "notifications/initialized" };
                using (var ackResponse = await PostAsync(initializedBody, sessionId, cancellationToken))
                {
                    // 202 Accepted očekivano; ne tretiramo kao grešku.
                }

                // 3) tools/call
                var callBody = new
                {
                    jsonrpc = "2.0",
                    id = 2,
                    method = "tools/call",
                    @params = new { name = toolName, arguments }
                };

                using var callResponse = await PostAsync(callBody, sessionId, cancellationToken);
                if (!callResponse.IsSuccessStatusCode)
                    throw new McpUnavailableException($"MCP tools/call neuspješan (HTTP {(int)callResponse.StatusCode}).");

                var raw = await callResponse.Content.ReadAsStringAsync(cancellationToken);
                using var doc = ParseJsonRpc(raw);
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out var error))
                {
                    var msg = error.TryGetProperty("message", out var m) ? m.GetString() : "nepoznata greška";
                    throw new McpUnavailableException($"MCP alat '{toolName}' je vratio grešku: {msg}");
                }

                if (!root.TryGetProperty("result", out var result) ||
                    !result.TryGetProperty("content", out var content) ||
                    content.GetArrayLength() == 0)
                {
                    throw new McpUnavailableException($"MCP alat '{toolName}' nije vratio sadržaj.");
                }

                var text = content[0].GetProperty("text").GetString() ?? "{}";
                using var inner = JsonDocument.Parse(text);
                return inner.RootElement.Clone();
            }
            catch (McpUnavailableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new McpUnavailableException("MCP server trenutno nije dostupan.", ex);
            }
        }

        private async Task<HttpResponseMessage> PostAsync(object body, string? sessionId, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(body, _jsonOpts);
            using var request = new HttpRequestMessage(HttpMethod.Post, _serverUrl)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Accept.ParseAdd("application/json");
            request.Headers.Accept.ParseAdd("text/event-stream");
            if (!string.IsNullOrEmpty(sessionId))
                request.Headers.TryAddWithoutValidation("Mcp-Session-Id", sessionId);

            return await _http.SendAsync(request, ct);
        }

        // Podržava i čisti JSON i SSE (text/event-stream) odgovor.
        private static JsonDocument ParseJsonRpc(string body)
        {
            var trimmed = body.TrimStart();
            if (trimmed.StartsWith('{'))
                return JsonDocument.Parse(trimmed);

            foreach (var line in body.Split('\n'))
            {
                var l = line.Trim();
                if (l.StartsWith("data:"))
                {
                    var data = l[5..].Trim();
                    if (data.StartsWith('{'))
                        return JsonDocument.Parse(data);
                }
            }
            throw new McpUnavailableException("Neočekivan format odgovora MCP servera.");
        }
    }
}
