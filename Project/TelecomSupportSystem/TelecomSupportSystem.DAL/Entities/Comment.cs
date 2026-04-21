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
        public int AuthorId { get; set; }
        public int TicketId { get; set; }

        public User Author { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}
