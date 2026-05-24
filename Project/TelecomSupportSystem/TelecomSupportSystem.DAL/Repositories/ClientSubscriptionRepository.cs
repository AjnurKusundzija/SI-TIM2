using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class ClientSubscriptionRepository : IClientSubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientSubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClientSubscription>> GetByClientIdAsync(int clientId)
            => await _context.ClientSubscriptions
                .Where(s => s.UserId == clientId)
                .Include(s => s.CatalogPackage)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

        public async Task<IEnumerable<ClientSubscription>> GetActiveByClientIdAsync(int clientId)
            => await _context.ClientSubscriptions
                .Where(s => s.UserId == clientId && s.Status == PackageStatus.ACTIVE)
                .Include(s => s.CatalogPackage)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();

        public async Task<ClientSubscription?> GetByIdAsync(int subscriptionId)
            => await _context.ClientSubscriptions
                .Include(s => s.CatalogPackage)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);

        public async Task<bool> HasActiveSubscriptionAsync(int clientId, int catalogPackageId)
            => await _context.ClientSubscriptions
                .AnyAsync(s => s.UserId == clientId
                            && s.CatalogPackageId == catalogPackageId
                            && s.Status == PackageStatus.ACTIVE);

        public async Task AddAsync(ClientSubscription subscription)
            => await _context.ClientSubscriptions.AddAsync(subscription);

        public Task UpdateAsync(ClientSubscription subscription)
        {
            _context.ClientSubscriptions.Update(subscription);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
