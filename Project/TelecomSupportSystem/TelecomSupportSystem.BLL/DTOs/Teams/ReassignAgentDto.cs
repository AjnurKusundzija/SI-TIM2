using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Teams
{
    public class ReassignAgentDto
    {
        [Required]
        public int AgentId { get; set; }

        [Required]
        public int NewTeamId { get; set; }
    }
}
