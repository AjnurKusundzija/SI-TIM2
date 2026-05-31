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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            var requestorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (requestorIdClaim is null || !int.TryParse(requestorIdClaim, out int requestorId) || role is null)
                return Unauthorized();

            if (role == "CLIENT" && id != requestorId)
                return Forbid();

            if (role != "CLIENT" && role != "AGENT" && role != "TECHNICIAN" && role != "ADMINISTRATOR")
                return Forbid();

            try
            {
                var profile = await _userService.GetUserProfileAsync(id, requestorId, role);
                return Ok(profile);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("{id:int}/statistics")]
        public async Task<IActionResult> GetUserStatistics(int id)
        {
            var requestorRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (requestorRole != "ADMINISTRATOR" && requestorRole != "AGENT")
                return Forbid();

            try
            {
                // Verify the user exists and check their role
                // GetUserProfileAsync handles auth and checks if user exists
                var requestorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var profile = await _userService.GetUserProfileAsync(id, requestorId, requestorRole);
                
                if (profile.Role != "AGENT" && profile.Role != "TECHNICIAN")
                    return BadRequest(new { message = "Statistika je dostupna samo za agente i tehničare." });

                var stats = await _userService.GetMyStatisticsAsync(id, profile.Role);
                return Ok(stats);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == null) return Unauthorized();

            if (currentRole != "ADMINISTRATOR")
                return Forbid();

            var currentUserId = TryGetUserId(out var parsedUserId) ? parsedUserId : (int?)null;
            var currentEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            try
            {
                await _userService.CreateUserAsync(dto, currentRole, currentUserId, currentEmail);
                return Ok(new { message = "Korisnik je uspješno kreiran." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUserDetails(int id, [FromBody] UpdateUserDetailsDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == null) return Unauthorized();

            if (currentRole != "ADMINISTRATOR" && currentRole != "AGENT")
                return Forbid();

            var currentUserId = TryGetUserId(out var parsedUserId) ? parsedUserId : (int?)null;

            try
            {
                await _userService.UpdateUserDetailsAsync(id, dto, currentRole, currentUserId);
                return Ok(new { message = "Podaci korisnika su uspješno ažurirani." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id:int}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            if (!TryGetUserId(out var currentUserId)) return Unauthorized();
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == null) return Unauthorized();

            if (currentRole != "ADMINISTRATOR" && currentRole != "AGENT")
                return Forbid();

            try
            {
                await _userService.ChangeUserStatusAsync(id, false, currentRole, currentUserId);
                return Ok(new { message = "Korisnik je uspješno deaktiviran." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}/reactivate")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            if (!TryGetUserId(out var currentUserId)) return Unauthorized();
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == null) return Unauthorized();

            if (currentRole != "ADMINISTRATOR")
                return Forbid();

            try
            {
                await _userService.ChangeUserStatusAsync(id, true, currentRole, currentUserId);
                return Ok(new { message = "Korisnik je uspješno reaktiviran." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUsersList([FromQuery] string? role, [FromQuery] string? status, [FromQuery] string? availability, [FromQuery] string? search, [FromQuery] string? location, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole == null) return Unauthorized();

            try
            {
                var result = await _userService.GetUsersPaginatedAsync(currentRole, role, status, availability, search, location, page, pageSize);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("me/availability")]
        public async Task<IActionResult> SetMyAvailability([FromBody] Dictionary<string,string> dto)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == null) return Unauthorized();

            // Expecting body: { availability: "AVAILABLE" }
            if (dto == null || !dto.TryGetValue("availability", out var availability))
                return BadRequest(new { message = "Neispravan payload." });

            try
            {
                await _userService.SetAvailabilityAsync(userId, availability, role, userId);
                return Ok(new { message = "Status dostupnosti je ažuriran." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Korisnik nije pronađen." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("agent-teams")]
        public async Task<IActionResult> GetAgentTeams()
        {
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (currentRole != "ADMINISTRATOR") return Forbid();

            var teams = await _userService.GetAgentTeamsAsync();
            return Ok(teams);
        }
    }
}
