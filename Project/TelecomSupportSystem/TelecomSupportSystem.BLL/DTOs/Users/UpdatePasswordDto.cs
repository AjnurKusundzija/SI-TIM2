using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UpdatePasswordDto
    {
        [Required(ErrorMessage = "Trenutna lozinka je obavezna.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nova lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Nova lozinka mora imati najmanje 8 znakova.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda nove lozinke je obavezna.")]
        [Compare("NewPassword", ErrorMessage = "Lozinke se ne poklapaju.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
