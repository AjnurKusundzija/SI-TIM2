// Lokacija: TelecomSupportSystem.BLL/DTOs/MyTicketDto.cs
namespace TelecomSupportSystem.BLL.DTOs
{
    public class MyTicketDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? InternalPriority { get; set; }
        public string ProblemCategory { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public bool HasAssignment { get; set; }

        // SLA (US-115) — null for CLOSED tickets
        public DateTime? SlaDeadline { get; set; }
        public double? SlaRemainingMinutes { get; set; }
        public string? SlaStatus { get; set; }
        public bool SlaIsBreached { get; set; }
    }
}