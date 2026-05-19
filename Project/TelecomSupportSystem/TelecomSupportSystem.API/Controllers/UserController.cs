using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private bool TryGetUserId(out int userId)
        {
            userId = 0;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return false;
            }

            return int.TryParse(userIdClaim, out userId);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var profile = await _userService.GetMyProfileAsync(userId);
            return Ok(profile);
        }

        [HttpPut("me/email")]
        public async Task<IActionResult> UpdateMyEmail([FromBody] UpdateEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized();

            try
            {
                await _userService.UpdateEmailAsync(userId, dto);
                return Ok(new { message = "Email adresa je uspješno ažurirana." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> UpdateMyPassword([FromBody] UpdatePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetUserId(out var userId))
                return Unauthorized();

            try
            {
                await _userService.UpdatePasswordAsync(userId, dto);
                return Ok(new { message = "Lozinka je uspješno promijenjena." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
        }

        // PB-42: GET /api/users/me/statistics — statistika rada za agenta ili tehničara
        [HttpGet("me/statistics")]
        public async Task<IActionResult> GetMyStatistics()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT" && role != "TECHNICIAN")
                return Forbid();

            var stats = await _userService.GetMyStatisticsAsync(userId, role);
            return Ok(stats);
        }

        // Dashboard: GET /api/users/me/recent-tickets — 5 najrecentnijih tiketa
        [HttpGet("me/recent-tickets")]
        public async Task<IActionResult> GetRecentTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT" && role != "TECHNICIAN")
                return Forbid();

            var tickets = await _userService.GetRecentAssignedTicketsAsync(userId);
            return Ok(tickets);
        }
    }
}
