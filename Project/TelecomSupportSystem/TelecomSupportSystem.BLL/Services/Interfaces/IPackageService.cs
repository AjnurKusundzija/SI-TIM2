using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.Packages;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IPackageService
    {
        // US-6: Lista aktivnih paketa za prijavljenog korisnika
        Task<IEnumerable<PackageSummaryDto>> GetMyPackagesAsync(int userId);

        // US-7: Detalji jednog paketa — paket mora pripadati korisniku
        Task<PackageDetailDto> GetPackageByIdAsync(int packageId, int userId);
    }
}
