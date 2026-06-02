namespace TelecomSupportSystem.BLL.DTOs.Teams
{
    public class TeamMemberDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ExpertiseCategory { get; set; } = string.Empty;
        public string? Availability { get; set; }
        public int OpenTicketCount { get; set; }
    }
}
