using System.ComponentModel.DataAnnotations;

namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class CreateCatalogPackageDto
    {
        [Required(ErrorMessage = "Naziv paketa je obavezan.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Naziv mora imati između 1 i 100 znakova.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tip paketa je obavezan.")]
        public string Type { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Opis ne smije biti duži od 1000 znakova.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti pozitivan broj.")]
        public decimal Price { get; set; }

        public string? Status { get; set; }
    }
}
