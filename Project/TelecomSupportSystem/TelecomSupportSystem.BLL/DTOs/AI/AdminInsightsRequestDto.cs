namespace TelecomSupportSystem.BLL.DTOs.AI
{
    public class AdminInsightsRequestDto
    {
        public string Period { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int ClosureRequestedTickets { get; set; }
        public int UnassignedTickets { get; set; }
        public int StaleTickets { get; set; }
        public double? AvgFirstResponseMinutes { get; set; }
        public double? AvgResolutionHours { get; set; }
        public double? AvgRating { get; set; }
        public List<CategoryCountDto> TopProblemCategories { get; set; } = [];
        public List<AgentWorkloadSnippetDto> TopAgentWorkload { get; set; } = [];
    }

    public class CategoryCountDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AgentWorkloadSnippetDto
    {
        public string AgentName { get; set; } = string.Empty;
        public int TicketCount { get; set; }
    }
}
