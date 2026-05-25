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

            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Content cannot be empty.");

            try
            {
                var fileUploads = new List<FileUploadDto>();

                if (request.Attachments is not null && request.Attachments.Count > 0)
                {
                    // US-80: Ograničenje na maksimalno 5 priloga po poruci
                    if (request.Attachments.Count > 5)
                    {
                        return BadRequest("Maksimalan broj priloga po jednoj poruci je 5.");
                    }

                    // Definišemo dozvoljene i strogo zabranjene ekstenzije fajlova
                    var allowedExtensions = new HashSet<string> { ".png", ".jpg", ".jpeg", ".pdf", ".docx", ".txt" };
                    var forbiddenExtensions = new HashSet<string> { ".exe", ".bat", ".sh", ".cmd", ".com" };

                    foreach (var file in request.Attachments)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        // US-80: Maksimalna veličina pojedinačnog fajla je 5 MB (5 * 1024 * 1024 bajta)
                        if (file.Length > 5242880)
                        {
                            return BadRequest($"Fajl '{file.FileName}' prelazi maksimalnu dozvoljenu veličinu od 5 MB.");
                        }

                        // US-80: Explicitna zabrana izvršnih fajlova zbog sigurnosti sistema
                        if (forbiddenExtensions.Contains(extension))
                        {
                            return BadRequest($"Upload izvršnih fajlova ({extension}) je najstrožije zabranjen.");
                        }

                        // US-80: Dozvoljavaju se samo slike i dokumenti navedeni u zahtjevima
                        if (!allowedExtensions.Contains(extension))
                        {
                            return BadRequest($"Format fajla '{extension}' nije podržan. Dozvoljeni formati: PNG, JPG, JPEG, PDF, DOCX, TXT.");
                        }

                        // US-80: Sanitizacija naziva fajla (uklanjanje razmaka i specijalnih karaktera)
                        var sanitizedFileName = SanitizeFileName(file.FileName);

                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);

                        fileUploads.Add(new FileUploadDto
                        {
                            FileName = sanitizedFileName,
                            ContentType = file.ContentType ?? "application/octet-stream",
                            Data = ms.ToArray(),
                            Size = file.Length // Proslijeđena veličina fajla koju servisi zahtijevaju
                        });
                    }
                }

                var commentDto = await _commentService.AddCommentAsync(ticketId, userId, role, request.Content, fileUploads);

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

        // Pomoćna metoda za čišćenje i sanitizaciju naziva fajlova
        private static string SanitizeFileName(string fileName)
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            // Dozvoli samo slova, brojeve, crtice i donje crte
            var cleanName = System.Text.RegularExpressions.Regex.Replace(nameWithoutExt, @"[^a-zA-Z0-9_\-]", "");

            // Zamjena naših karaktera da ne lome URL encoding
            cleanName = cleanName.Replace("š", "s").Replace("đ", "d").Replace("č", "c").Replace("ć", "c").Replace("ž", "z")
                                 .Replace("Š", "S").Replace("Đ", "D").Replace("Č", "C").Replace("Ć", "C").Replace("Ž", "Z");

            if (string.IsNullOrWhiteSpace(cleanName))
            {
                cleanName = "prilog_" + Guid.NewGuid().ToString().Substring(0, 8);
            }

            return cleanName + extension;
        }
    }
}