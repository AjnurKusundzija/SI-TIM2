using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class ForwardToAgentDto
    {
        [Required]
        public int TargetAgentId { get; set; }
    }
}
