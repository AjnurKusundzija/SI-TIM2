using System;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Attachment
    {
        // AppDbContext traži 'AttachmentId' umjesto 'Id'
        public int AttachmentId { get; set; }
        
        public string FileName { get; set; } = string.Empty;
        
        // AppDbContext traži 'StoredFileName'
        public string StoredFileName { get; set; } = string.Empty;
        
        public string FilePath { get; set; } = string.Empty;
        
        // AppDbContext traži 'Size' umjesto 'FileSize'
        public long Size { get; set; }
        
        public string ContentType { get; set; } = string.Empty;
        
        public DateTime UploadedAt { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public int? TicketId { get; set; }
        
        // AppDbContext traži 'CommentId' umjesto 'MessageId'
        public int? CommentId { get; set; }

        // --- Navigacijski property-ji koje Fluent API konfigurira u AppDbContext ---
        
        // Pretpostavljam da se klase zovu Ticket i Comment (ili Comment/Message zavisno od BLL-a)
        // Ako se klasa za komentar zove drugačije, npr. TicketComment, slobodno promijeni tip ovdje
        public virtual Ticket? Ticket { get; set; }
        public virtual Comment? Comment { get; set; }
    }
}