namespace TelecomSupportSystem.BLL.DTOs.Teams
{
    public class TeamOverviewDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string SpecializedCategory { get; set; } = string.Empty;
        public int ActiveAgentCount { get; set; }
        public int OpenTicketCount { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
    }
}
