using System.ComponentModel.DataAnnotations;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Tickets
{
    public class UpdateInternalPriorityDto
    {
        [Required]
        public InternalPriority Priority { get; set; }
    }
}
