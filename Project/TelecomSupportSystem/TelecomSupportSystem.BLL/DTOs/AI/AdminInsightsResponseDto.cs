namespace TelecomSupportSystem.BLL.DTOs.AI
{
    public class AdminInsightsResponseDto
    {
        public string Narrative { get; set; } = string.Empty;
        public List<InsightItemDto> Anomalies { get; set; } = [];
        public List<RecommendationDto> Recommendations { get; set; } = [];
    }

    public class InsightItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class RecommendationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? TeamFilter { get; set; }
    }
}
