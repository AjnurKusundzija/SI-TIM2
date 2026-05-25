using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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

        public class CreateCommentWithAttachmentsRequest
        {
            public string Content { get; set; } = string.Empty;
            public IFormFileCollection? Attachments { get; set; }
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

        // PB-27 (alternative): POST /api/comment/tickets/{ticketId}/attachments - multipart/form-data with files
        [HttpPost("tickets/{ticketId:int}/attachments")]
        [RequestSizeLimit(31457280)] // ~30MB ukupni limit za čitav request (dovoljno za 5 fajlova po 5MB + tekst)
        public async Task<IActionResult> AddCommentWithAttachments(int ticketId, [FromForm] CreateCommentWithAttachmentsRequest request, [FromServices] Microsoft.AspNetCore.SignalR.IHubContext<Hubs.ChatHub> hubContext)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            var hasAttachments = request.Attachments is not null && request.Attachments.Count > 0;
            // PB-56: dozvoli slanje samo priloga (bez teksta). Tekst je obavezan samo ako nema fajlova.
            if (string.IsNullOrWhiteSpace(request.Content) && !hasAttachments)
                return BadRequest("Poruka mora imati tekst ili barem jedan prilog.");

            try
            {
                // PB-56: validacija/sanitizacija je centralizovana u BLL (AttachmentStorage).
                // Controller samo čita fajlove u memoriju i prosljeđuje servisu.
                var fileUploads = new List<FileUploadDto>();
                if (request.Attachments is not null && request.Attachments.Count > 0)
                {
                    foreach (var file in request.Attachments)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        fileUploads.Add(new FileUploadDto
                        {
                            FileName = file.FileName,
                            ContentType = file.ContentType ?? "application/octet-stream",
                            Data = ms.ToArray(),
                            Size = file.Length
                        });
                    }
                }

                // Ako tekst nije unesen ali postoje prilozi, šaljemo prazan content kroz servis.
                var content = request.Content ?? string.Empty;
                var commentDto = await _commentService.AddCommentAsync(ticketId, userId, role, content, fileUploads);

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