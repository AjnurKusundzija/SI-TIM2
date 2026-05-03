using System.ComponentModel.DataAnnotations;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class CreateTicketDto
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public ProblemCategory Type { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Priority Priority { get; set; }
    }
}
