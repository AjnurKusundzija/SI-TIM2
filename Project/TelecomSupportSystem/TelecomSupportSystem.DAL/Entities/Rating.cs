using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int RatingValue { get; set; }
        public string RatingComment { get; set; } = string.Empty;
        public DateTime RatingDate { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }

        public User User { get; set; } = null!;
        public Ticket Ticket { get; set; } = null!;
    }
}
