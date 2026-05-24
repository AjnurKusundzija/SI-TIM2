using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class SubscriptionAuditLogRepository : ISubscriptionAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionAuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SubscriptionAuditLog entry)
            => await _context.SubscriptionAuditLogs.AddAsync(entry);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
