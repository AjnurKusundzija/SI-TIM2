using System;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Attachment
    {
        public int AttachmentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public long Size { get; set; }

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }

        // PB-56: korisnik koji je uploadovao prilog
        public int? UserId { get; set; }

        public int? TicketId { get; set; }

        public int? CommentId { get; set; }

        public virtual Ticket? Ticket { get; set; }
        public virtual Comment? Comment { get; set; }
        public virtual User? User { get; set; }
    }
}
