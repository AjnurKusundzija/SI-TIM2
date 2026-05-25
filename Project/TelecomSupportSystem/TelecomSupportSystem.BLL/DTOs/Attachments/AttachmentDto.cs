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
        public string DownloadUrl { get; set; } = string.Empty;

        // PB-56 / US-81: korisnik koji je uploadovao prilog
        public int? UploadedByUserId { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
    }
}
