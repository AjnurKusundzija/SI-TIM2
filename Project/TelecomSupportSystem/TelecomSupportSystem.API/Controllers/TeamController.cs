using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Teams;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/teams")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim is not null && int.TryParse(claim, out userId);
        }

        /// <summary>
        /// US-24: Returns overview of all teams with their active members and open ticket counts.
        /// Accessible by Administrators only.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> GetTeamsOverview()
        {
            try
            {
                var teams = await _teamService.GetAllTeamsOverviewAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Greška pri dohvatanju timova.", detail = ex.Message });
            }
        }

        /// <summary>
        /// US-24: Reassigns an agent to a new team.
        /// Blocked if agent is inactive or has open tickets (backend enforced).
        /// </summary>
        [HttpPost("reassign")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> ReassignAgent([FromBody] ReassignAgentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var adminId))
                return Unauthorized();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            try
            {
                await _teamService.ReassignAgentAsync(dto.AgentId, dto.NewTeamId, adminId, ipAddress);
                return Ok(new { message = "Agent je uspješno premješten u novi tim." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
