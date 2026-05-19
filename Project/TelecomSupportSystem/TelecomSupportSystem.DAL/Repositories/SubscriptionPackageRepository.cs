using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class SubscriptionPackageRepository : ISubscriptionPackageRepository
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionPackageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // US-6: WHERE UserId = userId AND PackageStatus = ACTIVE — garantuje da
        // korisnik nikada ne vidi tuđe pakete niti neaktivne pretplate.
        public async Task<IEnumerable<SubscriptionPackage>> GetActivePackagesByUserIdAsync(int userId)
        {
            return await _context.SubscriptionPackages
                .Where(p => p.UserId == userId && p.PackageStatus == PackageStatus.ACTIVE)
                .Include(p => p.Features)
                .OrderBy(p => p.PackageName)
                .ToListAsync();
        }

        // US-7: Detalji paketa zajedno sa svim uslugama (Features). Provjera vlasništva
        // se radi u servisnom sloju nakon učitavanja entiteta.
        public async Task<SubscriptionPackage?> GetByIdWithFeaturesAsync(int packageId)
        {
            return await _context.SubscriptionPackages
                .Include(p => p.Features)
                .FirstOrDefaultAsync(p => p.PackageId == packageId);
        }
    }
}
