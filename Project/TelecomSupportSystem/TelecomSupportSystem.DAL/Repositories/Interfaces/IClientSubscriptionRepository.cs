using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IClientSubscriptionRepository
    {
        Task<IEnumerable<ClientSubscription>> GetByClientIdAsync(int clientId);
        Task<IEnumerable<ClientSubscription>> GetActiveByClientIdAsync(int clientId);
        Task<ClientSubscription?> GetByIdAsync(int subscriptionId);
        Task<bool> HasActiveSubscriptionAsync(int clientId, int catalogPackageId);
        Task AddAsync(ClientSubscription subscription);
        Task UpdateAsync(ClientSubscription subscription);
        Task<int> SaveChangesAsync();
    }
}
