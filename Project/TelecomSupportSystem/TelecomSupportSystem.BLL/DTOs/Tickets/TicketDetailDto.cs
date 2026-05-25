namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class TicketDetailDto
    {
        public int TicketId { get; set; }
        public int CreatorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? InternalPriority { get; set; }
        public string ProblemCategory { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }

        // Closure Workflow
        public DateTime? ClosureRequestedDate { get; set; }
        public int? ClosureRequestedById { get; set; }
        public string? ClosureRequestStatus { get; set; }
        public int? ClosedById { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string AssignedAgentName { get; set; } = string.Empty;
        public int? AssignedAgentId { get; set; }
        public string AssignedTechnicianName { get; set; } = string.Empty;
        public int? AssignedTechnicianId { get; set; }
        public IEnumerable<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto> Attachments { get; set; } = new List<TelecomSupportSystem.BLL.DTOs.Attachments.AttachmentDto>();
    }
}
