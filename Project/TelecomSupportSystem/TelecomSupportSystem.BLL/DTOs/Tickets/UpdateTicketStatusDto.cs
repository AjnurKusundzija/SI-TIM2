using System.ComponentModel.DataAnnotations;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class UpdateTicketStatusDto
    {
        [Required]
        public TicketStatus Status { get; set; }
    }
}
