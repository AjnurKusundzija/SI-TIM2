namespace TelecomSupportSystem.BLL.DTOs.Notification
{
    public class GetNotifcationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public int? TicketId { get; set; }
    }
}
