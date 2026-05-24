using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class CatalogPackageRepository : ICatalogPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public CatalogPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CatalogPackage>> GetAllAsync()
            => await _context.CatalogPackages.OrderBy(p => p.Name).ToListAsync();

        public async Task<IEnumerable<CatalogPackage>> GetByStatusAsync(PackageStatus status)
            => await _context.CatalogPackages
                .Where(p => p.Status == status)
                .OrderBy(p => p.Name)
                .ToListAsync();

        public async Task<CatalogPackage?> GetByIdAsync(int id)
            => await _context.CatalogPackages.FirstOrDefaultAsync(p => p.CatalogPackageId == id);

        public async Task AddAsync(CatalogPackage package)
            => await _context.CatalogPackages.AddAsync(package);

        public Task UpdateAsync(CatalogPackage package)
        {
            _context.CatalogPackages.Update(package);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CatalogPackage package)
        {
            _context.CatalogPackages.Remove(package);
            return Task.CompletedTask;
        }

        public async Task<int> CountActiveSubscriptionsAsync(int catalogPackageId)
            => await _context.ClientSubscriptions
                .CountAsync(s => s.CatalogPackageId == catalogPackageId && s.Status == PackageStatus.ACTIVE);

        public async Task<IDictionary<int, int>> GetActiveSubscriptionCountsAsync()
            => await _context.ClientSubscriptions
                .Where(s => s.Status == PackageStatus.ACTIVE)
                .GroupBy(s => s.CatalogPackageId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
