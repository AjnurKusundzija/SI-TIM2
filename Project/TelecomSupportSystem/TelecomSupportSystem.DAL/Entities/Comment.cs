using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
        public bool IsInternal { get; set; }
        public bool IsSystemMessage { get; set; }
        public int? AuthorId { get; set; }
        public int TicketId { get; set; }

        public User? Author { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
