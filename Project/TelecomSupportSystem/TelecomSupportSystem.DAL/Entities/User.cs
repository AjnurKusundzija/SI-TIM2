using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public Role Role { get; set; }
        public AvailabilityStatus? AvailabilityStatus { get; set; }
        public int? TeamId { get; set; }

        public Team? Team { get; set; }
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<SubscriptionPackage> Packages { get; set; } = new List<SubscriptionPackage>();
        public ICollection<TicketUser> TicketAssignments { get; set; } = new List<TicketUser>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
