namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 — ključna metrika prikazana u odgovoru Admin Copilota.
    public class AdminCopilotMetricDto
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Hint { get; set; }
    }
}
