namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 / US-110, US-111 — preporuka. NE izvršava akcije automatski (samo prijedlog).
    public class AdminCopilotRecommendationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Opcioni filter (kategorija ili teamId) za drill-down ka tiketima.
        public string? TeamFilter { get; set; }
    }
}
