using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class ForwardToTechnicianDto
    {
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
