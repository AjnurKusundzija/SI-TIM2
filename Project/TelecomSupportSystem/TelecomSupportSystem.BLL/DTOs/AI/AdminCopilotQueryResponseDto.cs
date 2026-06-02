namespace TelecomSupportSystem.BLL.DTOs.AI
{
    // PB-70 / US-108, US-109, US-110, US-111 — strukturiran odgovor Admin Copilota.
    public class AdminCopilotQueryResponseDto
    {
        // Narativni odgovor (sažetak na bosanskom) — formatira Groq model.
        public string Answer { get; set; } = string.Empty;

        // Prepoznati intent: team_workload | faq_coverage | tickets_no_response | general_admin_question | unsupported
        public string Intent { get; set; } = string.Empty;

        public List<AdminCopilotMetricDto> Metrics { get; set; } = [];
        public List<AdminCopilotRecommendationDto> Recommendations { get; set; } = [];

        // US-109 — izvori i korišteni MCP alati.
        public List<AdminCopilotSourceDto> Sources { get; set; } = [];
        public List<string> UsedTools { get; set; } = [];

        // US-110 — relevantni tiketi (npr. otvoreni tiketi najopterećenijeg tima).
        public List<AdminCopilotRelatedTicketDto> RelatedTickets { get; set; } = [];

        // US-111 — pokrivenost ponavljanih problema FAQ-om + prijedlozi novih FAQ stavki.
        public List<AdminCopilotFaqCoverageDto> FaqCoverage { get; set; } = [];

        // Poruka za korisnika (npr. zahtjev za preciziranje, ili napomena o parcijalnim podacima).
        public string? Message { get; set; }
    }

    public class AdminCopilotRelatedTicketDto
    {
        public int TicketId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? TeamName { get; set; }
        public int? MinutesWithoutResponse { get; set; }
    }

    public class AdminCopilotFaqCoverageDto
    {
        public string Problem { get; set; } = string.Empty;
        public int OccurrenceCount { get; set; }
        public bool Covered { get; set; }
        public string? MatchedFaqQuestion { get; set; }

        // Prijedlog nove FAQ stavke (NE kreira se automatski — samo nacrt).
        public string? SuggestedQuestion { get; set; }
        public string? SuggestedAnswer { get; set; }
    }
}
