using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        // PB-57 / US-96: Agent/Technician requests an AI-generated response suggestion
        [HttpPost("agent-suggestion")]
        public async Task<IActionResult> GetAgentSuggestion([FromBody] AgentSuggestionRequestDto request)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "AGENT" && role != "TECHNICIAN")
                return Forbid();

            try
            {
                var result = await _aiService.GetAgentSuggestionAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message });
            }
            catch
            {
                return StatusCode(503, new { message = "AI servis trenutno nije dostupan. Pokušajte ponovo." });
            }
        }

        // PB-58 / US-98, US-99: Admin requests AI insights for the dashboard
        [HttpPost("admin-insights")]
        public async Task<IActionResult> GetAdminInsights([FromBody] AdminInsightsRequestDto request)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "ADMINISTRATOR")
                return Forbid();

            try
            {
                var result = await _aiService.GetAdminInsightsAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(503, new { message = ex.Message });
            }
            catch
            {
                return StatusCode(503, new { message = "AI servis trenutno nije dostupan. Pokušajte ponovo." });
            }
        }
    }
}
