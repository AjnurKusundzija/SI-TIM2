using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Download(int id)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment is null)
                return NotFound();

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            var fullPath = Path.Combine(folder, attachment.StoredFileName ?? string.Empty);
            if (!System.IO.File.Exists(fullPath))
                return NotFound();

            var contentType = string.IsNullOrWhiteSpace(attachment.ContentType) ? "application/octet-stream" : attachment.ContentType;
            return PhysicalFile(fullPath, contentType, attachment.FileName);
        }
    }
}
