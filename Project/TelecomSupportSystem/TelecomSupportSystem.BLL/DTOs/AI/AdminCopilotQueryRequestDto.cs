namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 / US-108 — zahtjev Admin Copilota: slobodan tekst + opcioni kontekst dashboarda.
    public class AdminCopilotQueryRequestDto
    {
        public string Question { get; set; } = string.Empty;

        // Opcioni vremenski opseg (ako frontend želi ograničiti analizu na period dashboarda).
        public DateTime? PeriodFrom { get; set; }
        public DateTime? PeriodTo { get; set; }

        // Opcioni trenutni filter/kontekst dashboarda (npr. "period=week").
        public string? DashboardContext { get; set; }
    }
}
