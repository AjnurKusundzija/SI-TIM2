using System;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class GetTicketDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string ProblemCategory { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int CreatorId { get; set; }
        public int? TeamId { get; set; }
        public string? AssignedAgentName { get; set; }
        public string? AssignmentMessage { get; set; }
        public IEnumerable<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto> Attachments { get; set; } = new List<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto>();

        // SLA (US-115) — null for CLOSED tickets
        public DateTime? SlaDeadline { get; set; }
        public double? SlaRemainingMinutes { get; set; }
        public string? SlaStatus { get; set; }
        public bool SlaIsBreached { get; set; }
    }
}
