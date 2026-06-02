using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.DTOs.AI;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    // PB-70 / US-108..US-111 — MCP Admin Copilot. Dostupno isključivo ADMINISTRATOR roli.
    [ApiController]
    [Route("api/ai/admin-copilot")]
    [Authorize]
    public class AdminCopilotController : ControllerBase
    {
        private readonly IAdminCopilotService _copilot;

        public AdminCopilotController(IAdminCopilotService copilot)
        {
            _copilot = copilot;
        }

        // POST /api/ai/admin-copilot/query
        [HttpPost("query")]
        public async Task<IActionResult> Query([FromBody] AdminCopilotQueryRequestDto request, CancellationToken cancellationToken)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "ADMINISTRATOR")
                return Forbid();

            if (request is null || string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { message = "Pitanje ne smije biti prazno." });

            try
            {
                var result = await _copilot.QueryAsync(request, cancellationToken);
                return Ok(result);
            }
            catch (McpUnavailableException ex)
            {
                // US-109 — kontrolisana greška kada MCP server nije dostupan.
                return StatusCode(503, new { message = $"MCP server nije dostupan: {ex.Message}" });
            }
            catch (InvalidOperationException ex)
            {
                // npr. GROQ_API_KEY_2 nije konfigurisan.
                return StatusCode(503, new { message = ex.Message });
            }
            catch
            {
                return StatusCode(503, new { message = "MCP Admin Copilot trenutno nije dostupan. Pokušajte ponovo." });
            }
        }
    }
}
