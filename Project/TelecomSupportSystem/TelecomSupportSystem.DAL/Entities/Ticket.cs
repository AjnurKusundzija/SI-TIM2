using System;
using System.Collections.Generic;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public TicketStatus Status { get; set; }
        public Priority Priority { get; set; }
        public InternalPriority? InternalPriority { get; set; }
        public ProblemCategory ProblemCategory { get; set; }

        // Closure Workflow
        public DateTime? ClosureRequestedDate { get; set; }
        public int? ClosureRequestedById { get; set; }
        public ClosureRequestStatus? ClosureRequestStatus { get; set; }
        public int? ClosedById { get; set; }

        public int CreatorId { get; set; }
        public int? TeamId { get; set; }

        public User Creator { get; set; } = null!;
        public Team? Team { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public Rating? Rating { get; set; }
        public ICollection<TicketUser> Assignments { get; set; } = new List<TicketUser>();
    }
}
