using System;
using System.Collections.Generic;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NotificationType NotificationType { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
