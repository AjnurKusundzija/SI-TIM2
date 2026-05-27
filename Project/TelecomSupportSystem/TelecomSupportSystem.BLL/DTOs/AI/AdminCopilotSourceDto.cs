namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 / US-109 — izvor podataka (MCP alat) korišten za odgovor.
    public class AdminCopilotSourceDto
    {
        public string Tool { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
