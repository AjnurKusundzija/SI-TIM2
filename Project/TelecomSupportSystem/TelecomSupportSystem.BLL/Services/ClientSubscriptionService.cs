using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.Subscriptions;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    // PB-52 / US-77: Dodjela i ukidanje pretplata, plus audit log za svaku promjenu.
    public class ClientSubscriptionService : IClientSubscriptionService
    {
        private readonly IClientSubscriptionRepository _subscriptionRepository;
        private readonly ICatalogPackageRepository _catalogRepository;
        private readonly ISubscriptionAuditLogRepository _auditRepository;
        private readonly IUserRepository _userRepository;

        public ClientSubscriptionService(
            IClientSubscriptionRepository subscriptionRepository,
            ICatalogPackageRepository catalogRepository,
            ISubscriptionAuditLogRepository auditRepository,
            IUserRepository userRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _catalogRepository = catalogRepository;
            _auditRepository = auditRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ClientSubscriptionDto>> GetByClientIdAsync(int clientId)
        {
            await EnsureClientExistsAsync(clientId);
            var subs = await _subscriptionRepository.GetByClientIdAsync(clientId);
            return subs.Select(MapToDto);
        }

        public async Task<ClientSubscriptionDto> AssignAsync(int clientId, AssignSubscriptionDto dto, int adminId)
        {
            await EnsureClientExistsAsync(clientId);

            var catalog = await _catalogRepository.GetByIdAsync(dto.CatalogPackageId)
                ?? throw new KeyNotFoundException($"Paket {dto.CatalogPackageId} nije pronađen.");

            if (catalog.Status != PackageStatus.ACTIVE)
                throw new InvalidOperationException("Neaktivan paket se ne može dodijeliti klijentu.");

            if (await _subscriptionRepository.HasActiveSubscriptionAsync(clientId, dto.CatalogPackageId))
                throw new InvalidOperationException("Klijent već ima aktivnu pretplatu na ovaj paket.");

            var subscription = new ClientSubscription
            {
                UserId = clientId,
                CatalogPackageId = dto.CatalogPackageId,
                StartDate = dto.StartDate,
                Status = PackageStatus.ACTIVE,
            };

            await _subscriptionRepository.AddAsync(subscription);
            await _subscriptionRepository.SaveChangesAsync();

            await _auditRepository.AddAsync(new SubscriptionAuditLog
            {
                UserId = clientId,
                AdminId = adminId,
                CatalogPackageId = dto.CatalogPackageId,
                SubscriptionId = subscription.SubscriptionId,
                Action = "ASSIGNED",
                Timestamp = DateTime.UtcNow,
            });
            await _auditRepository.SaveChangesAsync();

            subscription.CatalogPackage = catalog;
            return MapToDto(subscription);
        }

        public async Task<ClientSubscriptionDto> DeactivateAsync(int clientId, int subscriptionId, int adminId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId)
                ?? throw new KeyNotFoundException($"Pretplata {subscriptionId} nije pronađena.");

            if (subscription.UserId != clientId)
                throw new InvalidOperationException("Pretplata ne pripada navedenom klijentu.");

            if (subscription.Status == PackageStatus.INACTIVE)
                return MapToDto(subscription);

            subscription.Status = PackageStatus.INACTIVE;
            subscription.DeactivatedDate = DateTime.UtcNow;
            await _subscriptionRepository.UpdateAsync(subscription);
            await _subscriptionRepository.SaveChangesAsync();

            await _auditRepository.AddAsync(new SubscriptionAuditLog
            {
                UserId = clientId,
                AdminId = adminId,
                CatalogPackageId = subscription.CatalogPackageId,
                SubscriptionId = subscription.SubscriptionId,
                Action = "DEACTIVATED",
                Timestamp = DateTime.UtcNow,
            });
            await _auditRepository.SaveChangesAsync();

            return MapToDto(subscription);
        }

        private async Task EnsureClientExistsAsync(int clientId)
        {
            var user = await _userRepository.GetByIdAsync(clientId)
                ?? throw new KeyNotFoundException($"Klijent {clientId} nije pronađen.");
            if (user.Role != Role.CLIENT)
                throw new InvalidOperationException("Pretplate se mogu dodjeljivati samo klijentima.");
        }

        private static ClientSubscriptionDto MapToDto(ClientSubscription s) => new()
        {
            SubscriptionId = s.SubscriptionId,
            CatalogPackageId = s.CatalogPackageId,
            PackageName = s.CatalogPackage?.Name ?? string.Empty,
            PackageType = s.CatalogPackage?.Type.ToString() ?? string.Empty,
            PackageDescription = s.CatalogPackage?.Description ?? string.Empty,
            Price = s.CatalogPackage?.Price ?? 0m,
            StartDate = s.StartDate,
            DeactivatedDate = s.DeactivatedDate,
            Status = s.Status.ToString(),
        };
    }
}
