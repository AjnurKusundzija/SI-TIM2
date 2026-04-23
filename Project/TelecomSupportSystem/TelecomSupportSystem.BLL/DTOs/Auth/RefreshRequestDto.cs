using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Auth
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
