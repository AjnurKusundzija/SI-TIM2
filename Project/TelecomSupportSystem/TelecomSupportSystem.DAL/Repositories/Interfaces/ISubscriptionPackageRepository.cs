using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ISubscriptionPackageRepository
    {
        // US-6: Vraća sve aktivne pakete povezane sa korisnikom (filter po UserId u WHERE klauzuli)
        Task<IEnumerable<SubscriptionPackage>> GetActivePackagesByUserIdAsync(int userId);

        // US-7: Vraća detalje paketa sa svim uključenim uslugama (Features include)
        Task<SubscriptionPackage?> GetByIdWithFeaturesAsync(int packageId);
    }
}
