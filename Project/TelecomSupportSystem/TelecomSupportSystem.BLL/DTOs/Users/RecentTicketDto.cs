namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class RecentTicketDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime LastActivityDate { get; set; }
    }
}