using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.Packages;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class PackageService : IPackageService
    {
        private readonly ISubscriptionPackageRepository _packageRepository;
        private readonly IClientSubscriptionRepository? _clientSubscriptionRepository;

        public PackageService(ISubscriptionPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        // PB-52: dodatni konstruktor — uključuje pretplate iz kataloga u "Moji paketi"
        // tako da klijent vidi promjene odmah kad ih admin dodijeli (US-77 AC).
        public PackageService(
            ISubscriptionPackageRepository packageRepository,
            IClientSubscriptionRepository clientSubscriptionRepository)
            : this(packageRepository)
        {
            _clientSubscriptionRepository = clientSubscriptionRepository;
        }

        // PB-52: "Moji paketi" / "Aktivni paketi i pretplate" sada čita ISKLJUČIVO
        // ClientSubscriptions tabelu (US-77 dodjele). Legacy PB-21 SubscriptionPackage
        // tabela više nije izvor istine za klijentski view — admin upravlja sve preko
        // kataloga (US-76) i pretplate (US-77).
        public async Task<IEnumerable<PackageSummaryDto>> GetMyPackagesAsync(int userId)
        {
            if (_clientSubscriptionRepository is null)
                return Array.Empty<PackageSummaryDto>();

            var catalogSubs = await _clientSubscriptionRepository.GetActiveByClientIdAsync(userId);

            return catalogSubs
                .Where(s => s.CatalogPackage is not null)
                .Select(s => new PackageSummaryDto
                {
                    PackageId          = s.SubscriptionId,
                    PackageName        = s.CatalogPackage.Name,
                    PackageType        = s.CatalogPackage.Type.ToString(),
                    PackageStatus      = s.Status.ToString(),
                    MonthlyPrice       = s.CatalogPackage.Price,
                    PackageDescription = s.CatalogPackage.Description,
                    Summary            = s.CatalogPackage.Description,
                    IncludedServices   = BuildIncludedServicesForType(s.CatalogPackage.Type),
                    StartDate          = s.StartDate,
                })
                .ToList();
        }

        // PB-52: ID parametar je sada SubscriptionId (iz ClientSubscriptions). Provjera
        // vlasništva ostaje ista — pretplata mora pripadati prijavljenom korisniku.
        public async Task<PackageDetailDto> GetPackageByIdAsync(int subscriptionId, int userId)
        {
            if (_clientSubscriptionRepository is null)
                throw new KeyNotFoundException($"Pretplata {subscriptionId} nije pronađena.");

            var subscription = await _clientSubscriptionRepository.GetByIdAsync(subscriptionId);

            if (subscription is null || subscription.CatalogPackage is null)
                throw new KeyNotFoundException($"Pretplata {subscriptionId} nije pronađena.");

            if (subscription.UserId != userId)
                throw new UnauthorizedAccessException("Nemate pristup ovoj pretplati.");

            return new PackageDetailDto
            {
                PackageId          = subscription.SubscriptionId,
                PackageName        = subscription.CatalogPackage.Name,
                PackageType        = subscription.CatalogPackage.Type.ToString(),
                PackageStatus      = subscription.Status.ToString(),
                MonthlyPrice       = subscription.CatalogPackage.Price,
                PackageDescription = subscription.CatalogPackage.Description,
                StartDate          = subscription.StartDate,
                EndDate            = subscription.DeactivatedDate,
                Features           = new List<PackageFeatureDto>(),
            };
        }

        private static List<string> BuildIncludedServicesForType(PackageType type)
        {
            return type switch
            {
                PackageType.INTERNET => new List<string> { "Internet" },
                PackageType.TV       => new List<string> { "TV" },
                PackageType.MOBILE   => new List<string> { "Mobilni" },
                PackageType.BUNDLE   => new List<string> { "Internet", "TV", "Mobilni" },
                _                    => new List<string>(),
            };
        }
    }
}
