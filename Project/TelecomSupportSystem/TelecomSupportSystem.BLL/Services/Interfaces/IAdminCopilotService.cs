using TelecomSupportSystem.BLL.DTOs.AI;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    // PB-70 — MCP Admin Copilot orkestracioni sloj (intent -> MCP alati -> Groq formatiranje).
    public interface IAdminCopilotService
    {
        Task<AdminCopilotQueryResponseDto> QueryAsync(
            AdminCopilotQueryRequestDto request,
            CancellationToken cancellationToken = default);
    }
}
