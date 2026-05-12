namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class AgentScoreDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public double ScorePercent { get; set; }
        public int ResolvedInCategory { get; set; }
        public double AvgRating { get; set; }
        public int OpenTickets { get; set; }
    }
}