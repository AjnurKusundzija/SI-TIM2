using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Security.Claims;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/attachments")]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentRepository _attachmentRepository;

        public AttachmentsController(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        // PB-56 / US-81: Download priloga uz provjeru prava pristupa prema roli korisnika.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || string.IsNullOrEmpty(role))
                return Unauthorized();

            var attachment = await _attachmentRepository.GetByIdWithRelationsAsync(id);
            if (attachment is null)
                return NotFound();

            // Prilog je vezan ili direktno za tiket ili indirektno kroz komentar.
            var ticket = attachment.Ticket ?? attachment.Comment?.Ticket;
            if (ticket is null)
                return NotFound();

            if (!HasAccess(ticket, userId, role))
                return Forbid();

            var fullPath = ResolveFullPath(attachment);
            if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                ? "application/octet-stream"
                : attachment.ContentType;
            return PhysicalFile(fullPath, contentType, attachment.FileName);
        }

        private static bool HasAccess(Ticket ticket, int userId, string role) => role switch
        {
            "ADMINISTRATOR" => true,
            "CLIENT"        => ticket.CreatorId == userId,
            // AGENT/TECHNICIAN moraju biti (ili biti bili) dodijeljeni na tiket
            "AGENT" or "TECHNICIAN" => ticket.Assignments.Any(a => a.UserId == userId),
            _ => false
        };

        private static string ResolveFullPath(Attachment attachment)
        {
            if (!string.IsNullOrWhiteSpace(attachment.FilePath) && System.IO.File.Exists(attachment.FilePath))
                return attachment.FilePath;

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            return Path.Combine(folder, attachment.StoredFileName ?? string.Empty);
        }
    }
}
