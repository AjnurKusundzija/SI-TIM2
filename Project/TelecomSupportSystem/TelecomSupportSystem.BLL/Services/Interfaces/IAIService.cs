using TelecomSupportSystem.BLL.DTOs.AI;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IAIService
    {
        Task<AgentSuggestionResponseDto> GetAgentSuggestionAsync(AgentSuggestionRequestDto request);
        Task<AdminInsightsResponseDto> GetAdminInsightsAsync(AdminInsightsRequestDto request);
    }
}
