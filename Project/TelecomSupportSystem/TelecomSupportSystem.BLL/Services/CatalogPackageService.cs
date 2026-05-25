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
    // PB-52 / US-76: CRUD nad katalogom + zaštita brisanja kada postoje aktivne pretplate.
    public class CatalogPackageService : ICatalogPackageService
    {
        private readonly ICatalogPackageRepository _repository;
        private readonly IAuditLogService? _auditLogService;

        public CatalogPackageService(ICatalogPackageRepository repository, IAuditLogService? auditLogService = null)
        {
            _repository = repository;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<CatalogPackageDto>> GetCatalogAsync()
        {
            var packages = await _repository.GetAllAsync();
            var counts = await _repository.GetActiveSubscriptionCountsAsync();
            return packages.Select(p => MapToDto(p, counts.TryGetValue(p.CatalogPackageId, out var c) ? c : 0));
        }

        public async Task<IEnumerable<CatalogPackageDto>> GetActiveCatalogAsync()
        {
            var packages = await _repository.GetByStatusAsync(PackageStatus.ACTIVE);
            var counts = await _repository.GetActiveSubscriptionCountsAsync();
            return packages.Select(p => MapToDto(p, counts.TryGetValue(p.CatalogPackageId, out var c) ? c : 0));
        }

        public async Task<CatalogPackageDto> CreateAsync(CreateCatalogPackageDto dto, int? adminId = null)
        {
            ValidateNameAndPrice(dto.Name, dto.Price);
            var type = ParseType(dto.Type);
            var status = ParseStatus(dto.Status) ?? PackageStatus.ACTIVE;

            var entity = new CatalogPackage
            {
                Name = dto.Name.Trim(),
                Type = type,
                Description = dto.Description?.Trim() ?? string.Empty,
                Price = dto.Price,
                Status = status,
                CreatedDate = DateTime.UtcNow,
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            if (_auditLogService is not null)
            {
                await _auditLogService.LogAsync(
                    AuditActionType.PACKAGE_CREATED,
                    "CatalogPackage",
                    entity.CatalogPackageId.ToString(),
                    $"Paket '{entity.Name}' kreiran",
                    userId: adminId,
                    newValue: new { name = entity.Name, type = entity.Type.ToString(), description = entity.Description, price = entity.Price, status = entity.Status.ToString() });
            }
            return MapToDto(entity, 0);
        }

        public async Task<CatalogPackageDto> UpdateAsync(int id, UpdateCatalogPackageDto dto, int? adminId = null)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Paket {id} nije pronađen.");

            ValidateNameAndPrice(dto.Name, dto.Price);
            var oldValue = new { name = entity.Name, type = entity.Type.ToString(), description = entity.Description, price = entity.Price, status = entity.Status.ToString() };
            entity.Name = dto.Name.Trim();
            entity.Type = ParseType(dto.Type);
            entity.Description = dto.Description?.Trim() ?? string.Empty;
            entity.Price = dto.Price;
            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = ParseStatus(dto.Status) ?? entity.Status;
            entity.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            if (_auditLogService is not null)
            {
                await _auditLogService.LogAsync(
                    AuditActionType.PACKAGE_UPDATED,
                    "CatalogPackage",
                    entity.CatalogPackageId.ToString(),
                    $"Paket '{entity.Name}' ažuriran",
                    userId: adminId,
                    oldValue: oldValue,
                    newValue: new { name = entity.Name, type = entity.Type.ToString(), description = entity.Description, price = entity.Price, status = entity.Status.ToString() });
            }

            var count = await _repository.CountActiveSubscriptionsAsync(id);
            return MapToDto(entity, count);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Paket {id} nije pronađen.");

            var activeCount = await _repository.CountActiveSubscriptionsAsync(id);
            if (activeCount > 0)
                throw new InvalidOperationException(
                    $"Paket ima {activeCount} aktivnih pretplata i ne može biti obrisan.");

            await _repository.DeleteAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task<CatalogPackageDto> UpdateStatusAsync(int id, string status, int? adminId = null)
        {
            var entity = await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Paket {id} nije pronađen.");

            var parsed = ParseStatus(status)
                ?? throw new ArgumentException("Nepoznat status paketa.", nameof(status));

            var oldStatus = entity.Status;
            entity.Status = parsed;
            entity.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            if (_auditLogService is not null && oldStatus != entity.Status)
            {
                await _auditLogService.LogAsync(
                    entity.Status == PackageStatus.INACTIVE ? AuditActionType.PACKAGE_DEACTIVATED : AuditActionType.PACKAGE_UPDATED,
                    "CatalogPackage",
                    entity.CatalogPackageId.ToString(),
                    entity.Status == PackageStatus.INACTIVE ? $"Paket '{entity.Name}' deaktiviran" : $"Status paketa '{entity.Name}' promijenjen",
                    userId: adminId,
                    oldValue: new { status = oldStatus.ToString() },
                    newValue: new { status = entity.Status.ToString() });
            }

            var count = await _repository.CountActiveSubscriptionsAsync(id);
            return MapToDto(entity, count);
        }

        private static void ValidateNameAndPrice(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Naziv paketa ne smije biti prazan.", nameof(name));

            if (price <= 0)
                throw new ArgumentException("Cijena mora biti pozitivan broj.", nameof(price));
        }

        private static PackageType ParseType(string raw)
        {
            if (Enum.TryParse<PackageType>(raw, ignoreCase: true, out var type))
                return type;
            throw new ArgumentException($"Nepoznat tip paketa: '{raw}'.", nameof(raw));
        }

        private static PackageStatus? ParseStatus(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            if (Enum.TryParse<PackageStatus>(raw, ignoreCase: true, out var status))
                return status;
            return null;
        }

        private static CatalogPackageDto MapToDto(CatalogPackage p, int activeCount) => new()
        {
            CatalogPackageId = p.CatalogPackageId,
            Name = p.Name,
            Type = p.Type.ToString(),
            Description = p.Description,
            Price = p.Price,
            Status = p.Status.ToString(),
            ActiveSubscriptionCount = activeCount,
        };
    }
}
