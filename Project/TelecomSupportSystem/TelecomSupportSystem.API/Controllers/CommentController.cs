using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // US-15: GET /api/comment/ticket/{ticketId}
        // Vraća historiju komentara za tiket. Slanje poruka će biti implementirano
        // putem SignalR Hub-a (PB-27).
        [HttpGet("ticket/{ticketId:int}")]
        public async Task<IActionResult> GetCommentsForTicket(int ticketId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                var comments = await _commentService.GetCommentsForTicketAsync(ticketId, userId, role);
                return Ok(comments);
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
    }
}