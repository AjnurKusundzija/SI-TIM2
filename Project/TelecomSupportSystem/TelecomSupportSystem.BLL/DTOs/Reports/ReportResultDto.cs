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

    public class TicketCountReportDto
    {
        public int TotalCount { get; set; }
    }

    public class TicketStatusReportDto
    {
        public IReadOnlyList<StatusCountDto> Items { get; set; } = Array.Empty<StatusCountDto>();
    }

    public class ProblemTypeReportDto
    {
        public IReadOnlyList<NamedCountDto> Items { get; set; } = Array.Empty<NamedCountDto>();
    }

    public class TeamWorkloadReportDto
    {
        public IReadOnlyList<AgentWorkloadDto> Items { get; set; } = Array.Empty<AgentWorkloadDto>();
    }

    public class UserRatingsReportDto
    {
        public double? AverageRating { get; set; }
        public int RatedTicketsCount { get; set; }
        public IReadOnlyList<RatingDistributionDto> Distribution { get; set; } = Array.Empty<RatingDistributionDto>();
    }

    public class RatingDistributionDto
    {
        public int Stars { get; set; }
        public int Count { get; set; }
    }

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
}
