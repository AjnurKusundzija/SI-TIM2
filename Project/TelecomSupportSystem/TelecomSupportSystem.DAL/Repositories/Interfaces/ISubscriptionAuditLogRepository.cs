using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ISubscriptionAuditLogRepository
    {
        Task AddAsync(SubscriptionAuditLog entry);
        Task<int> SaveChangesAsync();
    }
}
