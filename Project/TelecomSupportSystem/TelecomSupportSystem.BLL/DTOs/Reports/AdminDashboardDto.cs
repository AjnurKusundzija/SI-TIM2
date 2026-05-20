namespace TelecomSupportSystem.BLL.DTOs.Reports
{
    public class AdminDashboardDto
    {
        public ReportDateRangeDto Period { get; set; } = new();

        public int TotalTicketsInPeriod { get; set; }
        public IReadOnlyList<StatusCountDto> StatusBreakdown { get; set; } = Array.Empty<StatusCountDto>();
        public double? AvgFirstResponseMinutes { get; set; }
        public string FirstResponseBucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<FirstResponseBucketDto> FirstResponseByPeriod { get; set; } = Array.Empty<FirstResponseBucketDto>();
        public double? AvgResolutionHours { get; set; }
        public double? AvgRating { get; set; }
        public IReadOnlyList<NamedCountDto> TopProblemTypes { get; set; } = Array.Empty<NamedCountDto>();
        public IReadOnlyList<AgentWorkloadDto> TopAgentWorkload { get; set; } = Array.Empty<AgentWorkloadDto>();

        public UserRoleCountsDto ActiveUsersByRole { get; set; } = new();
        public int OpenTicketsCount { get; set; }
        public int ClosureRequestedCount { get; set; }
        public int UnassignedOpenCount { get; set; }
        public int StaleTicketsCount { get; set; }
    }

    public class StatusCountDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class NamedCountDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AgentWorkloadDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int ResolvedCount { get; set; }
    }

    public class UserRoleCountsDto
    {
        public int Clients { get; set; }
        public int Agents { get; set; }
        public int Technicians { get; set; }
        public int Administrators { get; set; }
    }
}
