namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class AllTicketsItemDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string ProblemCategory { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int CreatorId { get; set; }
        public string CreatorName { get; set; } = string.Empty;
    }
}
