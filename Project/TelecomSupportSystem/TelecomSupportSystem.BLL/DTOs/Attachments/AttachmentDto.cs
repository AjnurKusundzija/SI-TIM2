using System;

namespace TelecomSupportSystem.BLL.DTOs.Attachments
{
    public class AttachmentDto
    {
        public int AttachmentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string DownloadUrl { get; set; } = string.Empty; // Dodano polje koje servisi traže
    }
}