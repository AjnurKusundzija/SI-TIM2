using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Ratings;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId:int}/rating")]
    [Authorize]
    public class TicketRatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public TicketRatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // US-61: GET /api/tickets/{ticketId}/rating — klijent (kreator) i staff vide ocjenu
        [HttpGet]
        public async Task<IActionResult> GetRating(int ticketId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                var rating = await _ratingService.GetRatingForTicketAsync(ticketId, userId, role);
                if (rating is null)
                    return NoContent();

                return Ok(rating);
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

        // US-61: POST /api/tickets/{ticketId}/rating — samo CLIENT koji je kreator tiketa
        [HttpPost]
        public async Task<IActionResult> CreateRating(int ticketId, [FromBody] CreateRatingDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                var rating = await _ratingService.CreateRatingAsync(ticketId, userId, role, dto);
                return CreatedAtAction(nameof(GetRating), new { ticketId }, rating);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { poruka = ex.Message });
            }
        }
    }
}
