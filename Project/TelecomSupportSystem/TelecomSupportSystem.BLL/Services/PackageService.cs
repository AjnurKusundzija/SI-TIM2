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

        public PackageService(ISubscriptionPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        // US-6: Repozitorij već filtrira po UserId i statusu ACTIVE.
        // Sažetak (Summary) se generiše na osnovu tipa paketa i njegovih značajki.
        public async Task<IEnumerable<PackageSummaryDto>> GetMyPackagesAsync(int userId)
        {
            var packages = await _packageRepository.GetActivePackagesByUserIdAsync(userId);

            return packages.Select(p => new PackageSummaryDto
            {
                PackageId          = p.PackageId,
                PackageName        = p.PackageName,
                PackageType        = p.PackageType.ToString(),
                PackageStatus      = p.PackageStatus.ToString(),
                MonthlyPrice       = p.MonthlyPrice,
                PackageDescription = p.PackageDescription,
                Summary            = BuildSummary(p),
                IncludedServices   = BuildIncludedServices(p),
            });
        }

        // US-7: Provjera vlasništva — ako paket ne pripada korisniku → 403 (UnauthorizedAccessException).
        public async Task<PackageDetailDto> GetPackageByIdAsync(int packageId, int userId)
        {
            var package = await _packageRepository.GetByIdWithFeaturesAsync(packageId);

            if (package is null)
                throw new KeyNotFoundException($"Paket {packageId} nije pronađen.");

            if (package.UserId != userId)
                throw new UnauthorizedAccessException("Nemate pristup ovom paketu.");

            return new PackageDetailDto
            {
                PackageId          = package.PackageId,
                PackageName        = package.PackageName,
                PackageType        = package.PackageType.ToString(),
                PackageStatus      = package.PackageStatus.ToString(),
                MonthlyPrice       = package.MonthlyPrice,
                PackageDescription = package.PackageDescription,
                StartDate          = package.StartDate,
                EndDate            = package.EndDate,
                Features           = package.Features
                    .OrderBy(f => f.FeatureId)
                    .Select(f => new PackageFeatureDto
                    {
                        FeatureId   = f.FeatureId,
                        Name        = f.Name,
                        Value       = f.Value,
                        Unit        = f.Unit,
                        Description = f.Description,
                    })
                    .ToList(),
            };
        }

        // Generiše kratak prikaz koji se koristi na kartici u listi paketa.
        private static string BuildSummary(SubscriptionPackage p)
        {
            if (p.PackageType == PackageType.BUNDLE)
            {
                var types = new List<string>();
                if (p.Features.Any(f => f.Name.Contains("Internet", StringComparison.OrdinalIgnoreCase))) types.Add("Internet");
                if (p.Features.Any(f => f.Name.Contains("Kanal", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("TV", StringComparison.OrdinalIgnoreCase))) types.Add("TV");
                if (p.Features.Any(f => f.Name.Contains("Mobilni", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("Minut", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("SMS", StringComparison.OrdinalIgnoreCase))) types.Add("Mobilni");
                return types.Count > 0 ? string.Join(" + ", types) : "Kombinovani paket";
            }

            var primary = p.Features.FirstOrDefault();
            if (primary is null) return p.PackageDescription;

            var unit = string.IsNullOrWhiteSpace(primary.Unit) ? string.Empty : $" {primary.Unit}";
            return $"{primary.Value}{unit}";
        }

        private static List<string> BuildIncludedServices(SubscriptionPackage p)
        {
            return p.PackageType switch
            {
                PackageType.INTERNET => new List<string> { "Internet" },
                PackageType.TV       => new List<string> { "TV" },
                PackageType.MOBILE   => new List<string> { "Mobilni" },
                PackageType.BUNDLE   => InferBundleServices(p),
                _                    => new List<string>(),
            };
        }

        private static List<string> InferBundleServices(SubscriptionPackage p)
        {
            var services = new List<string>();
            if (p.Features.Any(f => f.Name.Contains("Internet", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("Brzina", StringComparison.OrdinalIgnoreCase))) services.Add("Internet");
            if (p.Features.Any(f => f.Name.Contains("Kanal", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("TV", StringComparison.OrdinalIgnoreCase))) services.Add("TV");
            if (p.Features.Any(f => f.Name.Contains("Mobilni", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("Minut", StringComparison.OrdinalIgnoreCase) || f.Name.Contains("SMS", StringComparison.OrdinalIgnoreCase))) services.Add("Mobilni");
            return services;
        }
    }
}
