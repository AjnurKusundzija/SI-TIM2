using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.Packages;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ICatalogPackageService
    {
        // US-76: Lista svih paketa za admina (uključuje neaktivne + broj aktivnih pretplata)
        Task<IEnumerable<CatalogPackageDto>> GetCatalogAsync();

        // US-76: Lista samo aktivnih paketa (klijentski / dropdown prilikom dodjele)
        Task<IEnumerable<CatalogPackageDto>> GetActiveCatalogAsync();

        // US-76
        Task<CatalogPackageDto> CreateAsync(CreateCatalogPackageDto dto, int? adminId = null);

        // US-76
        Task<CatalogPackageDto> UpdateAsync(int id, UpdateCatalogPackageDto dto, int? adminId = null);

        // US-76 — bacanje InvalidOperationException kada paket ima aktivne pretplate
        Task DeleteAsync(int id);

        // US-76 — aktivacija / deaktivacija
        Task<CatalogPackageDto> UpdateStatusAsync(int id, string status, int? adminId = null);
    }
}
