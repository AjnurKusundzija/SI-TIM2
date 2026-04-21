using System;
using System.Collections.Generic;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TeamType TeamType { get; set; }
        public TeamStatus TeamStatus { get; set; }

        public ICollection<User> Members { get; set; } = new List<User>();
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<TicketUser> Assignments { get; set; } = new List<TicketUser>();
    }
}
