namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 / US-108 — pojedinačna poruka u chat formatu (uloga + sadržaj).
    public class AdminCopilotMessageDto
    {
        public string Role { get; set; } = "assistant"; // "user" | "assistant"
        public string Content { get; set; } = string.Empty;
    }
}
