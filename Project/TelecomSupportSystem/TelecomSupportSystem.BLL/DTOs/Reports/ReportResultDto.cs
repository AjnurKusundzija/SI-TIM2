namespace TelecomSupportSystem.BLL.DTOs.Reports
{
    public class ReportResultDto
    {
        public string ReportType { get; set; } = string.Empty;
        public ReportDateRangeDto Period { get; set; } = new();
        public bool HasData { get; set; }
        public string? Message { get; set; }
        public bool ShowLargePeriodWarning { get; set; }
        public object? Data { get; set; }
    }

    // ── TICKET_COUNT ─────────────────────────────────────────────────────────────
    public class CountBucketDto
    {
        public string Label { get; set; } = string.Empty;
        public int TicketCount { get; set; }
    }

    public class TicketCountReportDto
    {
        public int TotalCount { get; set; }
        public string BucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<CountBucketDto> Buckets { get; set; } = Array.Empty<CountBucketDto>();
    }

    // ── TICKET_STATUS ────────────────────────────────────────────────────────────
    public class TicketStatusReportDto
    {
        public IReadOnlyList<StatusCountDto> Items { get; set; } = Array.Empty<StatusCountDto>();
    }

    // ── PROBLEM_TYPE ─────────────────────────────────────────────────────────────
    public class ProblemTypeReportDto
    {
        public IReadOnlyList<NamedCountDto> Items { get; set; } = Array.Empty<NamedCountDto>();
    }

    // ── TEAM_WORKLOAD ────────────────────────────────────────────────────────────
    public class WorkloadPeriodRowDto
    {
        public string Label { get; set; } = string.Empty;
        public IReadOnlyList<int> Counts { get; set; } = Array.Empty<int>();
    }

    public class TeamWorkloadReportDto
    {
        public IReadOnlyList<AgentWorkloadDto> Items { get; set; } = Array.Empty<AgentWorkloadDto>();
        public string BucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<string> AgentNames { get; set; } = Array.Empty<string>();
        public IReadOnlyList<WorkloadPeriodRowDto> PeriodRows { get; set; } = Array.Empty<WorkloadPeriodRowDto>();
    }

    // ── USER_RATINGS ─────────────────────────────────────────────────────────────
    public class RatingDistributionDto
    {
        public int Stars { get; set; }
        public int Count { get; set; }
    }

    public class RatingBucketDto
    {
        public string Label { get; set; } = string.Empty;
        public double? AvgRating { get; set; }
        public int Count { get; set; }
    }

    public class UserRatingsReportDto
    {
        public double? AverageRating { get; set; }
        public int RatedTicketsCount { get; set; }
        public IReadOnlyList<RatingDistributionDto> Distribution { get; set; } = Array.Empty<RatingDistributionDto>();
        public string BucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<RatingBucketDto> Buckets { get; set; } = Array.Empty<RatingBucketDto>();
    }

    // ── FIRST_RESPONSE ───────────────────────────────────────────────────────────
    public class FirstResponseReportDto
    {
        public double? AvgFirstResponseMinutes { get; set; }
        public int TotalTicketsCount { get; set; }
        public int TicketsWithResponseCount { get; set; }
        public string BucketGranularity { get; set; } = string.Empty;
        public string BucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<FirstResponseBucketDto> Buckets { get; set; } = Array.Empty<FirstResponseBucketDto>();
    }

    public class FirstResponseBucketDto
    {
        public string Label { get; set; } = string.Empty;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TicketCount { get; set; }
        public int TicketsWithResponseCount { get; set; }
        public double? AvgFirstResponseMinutes { get; set; }
    }

    // ── AVG_RESOLUTION ───────────────────────────────────────────────────────────
    public class ResolutionBucketDto
    {
        public string Label { get; set; } = string.Empty;
        public int TicketCount { get; set; }
        public int ClosedCount { get; set; }
        public double? AvgResolutionHours { get; set; }
    }

    public class AvgResolutionReportDto
    {
        public double? AvgResolutionHours { get; set; }
        public int ClosedTicketsCount { get; set; }
        public int TotalTicketsCount { get; set; }
        public string BucketGranularityLabel { get; set; } = string.Empty;
        public IReadOnlyList<ResolutionBucketDto> Buckets { get; set; } = Array.Empty<ResolutionBucketDto>();
    }
}
