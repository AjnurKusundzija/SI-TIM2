using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class UpdatePackageStatusDto
    {
        [Required(ErrorMessage = "Status je obavezan.")]
        public string Status { get; set; } = string.Empty;
    }
}
