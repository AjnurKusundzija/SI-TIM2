using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

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

        // US-15: GET /api/comment/tickets/{ticketId}
        // Vraća historiju komentara za tiket. Slanje poruka će biti implementirano
        // putem SignalR Hub-a (PB-27).
        [HttpGet("tickets/{ticketId:int}")]
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

        public class CreateCommentRequest
        {
            public string Content { get; set; } = string.Empty;
        }

        // PB-27: POST /api/comment/tickets/{ticketId}
        [HttpPost("tickets/{ticketId:int}")]
        public async Task<IActionResult> AddComment(int ticketId, [FromBody] CreateCommentRequest request, [FromServices] Microsoft.AspNetCore.SignalR.IHubContext<Hubs.ChatHub> hubContext)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content cannot be empty.");

            try
            {
                var commentDto = await _commentService.AddCommentAsync(ticketId, userId, role, request.Content);

                // Emit to SignalR group
                await hubContext.Clients.Group($"ticket_{ticketId}").SendAsync("ReceiveComment", commentDto);

                return Ok(commentDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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