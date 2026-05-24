using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ICatalogPackageRepository
    {
        Task<IEnumerable<CatalogPackage>> GetAllAsync();
        Task<IEnumerable<CatalogPackage>> GetByStatusAsync(PackageStatus status);
        Task<CatalogPackage?> GetByIdAsync(int id);
        Task AddAsync(CatalogPackage package);
        Task UpdateAsync(CatalogPackage package);
        Task DeleteAsync(CatalogPackage package);
        Task<int> CountActiveSubscriptionsAsync(int catalogPackageId);
        Task<IDictionary<int, int>> GetActiveSubscriptionCountsAsync();
        Task<int> SaveChangesAsync();
    }
}
