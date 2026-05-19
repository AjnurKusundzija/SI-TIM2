using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UpdateEmailDto
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        public string Email { get; set; } = string.Empty;
    }
}
