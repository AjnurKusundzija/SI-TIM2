using System.Text.Json;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    // PB-70 / US-109 — klijent koji poziva read-only alate MCP servera preko MCP protokola.
    public interface IMcpClient
    {
        /// <summary>
        /// Poziva MCP alat (npr. "team.workload") i vraća parsirani JSON izlaz alata.
        /// Baca <see cref="McpUnavailableException"/> ako MCP server nije dostupan.
        /// </summary>
        Task<JsonElement> CallToolAsync(string toolName, object arguments, CancellationToken cancellationToken = default);
    }

    public class McpUnavailableException : Exception
    {
        public McpUnavailableException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
