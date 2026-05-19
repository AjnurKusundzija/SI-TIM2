using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UpdateEmailDto
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.com$", 
            ErrorMessage = "Neispravan format. Email mora sadržavati '@' i završavati se sa '.com'")]
        public string Email { get; set; } = string.Empty;
    }
}