using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
