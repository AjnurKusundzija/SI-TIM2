using System.ComponentModel.DataAnnotations;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UpdateUserDetailsDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public Location? Location { get; set; }

        public int? TeamId { get; set; }
    }
}
