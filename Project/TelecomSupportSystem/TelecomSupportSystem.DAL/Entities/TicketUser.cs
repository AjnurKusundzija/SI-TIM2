using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class TicketUser
    {
        public int AssignmentId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public AssignmentType AssignmentType { get; set; }
        public string Note { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }

        public Ticket Ticket { get; set; } = null!;
        public User User { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
}
