using System.ComponentModel.DataAnnotations;
using TelecomSupportSystem.BLL.Helpers;

namespace TelecomSupportSystem.BLL.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required]
        [EmailOrBiHPhone]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
