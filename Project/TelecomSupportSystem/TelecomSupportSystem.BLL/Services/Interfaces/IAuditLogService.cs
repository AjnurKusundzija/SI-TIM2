using TelecomSupportSystem.BLL.DTOs.AuditLogs;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    /// <summary>
    /// Service za logiranje audit akcija u bazu podataka.
    /// </summary>
    public interface IAuditLogService
    {
        /// <summary>
        /// Logira akciju u audit log.
        /// </summary>
        Task LogAsync(
            AuditActionType actionType,
            string entityType,
            string? entityId,
            string description,
            int? userId = null,
            object? oldValue = null,
            object? newValue = null,
            string? ipAddress = null
        );

        /// <summary>
        /// Dohvata audit logove sa paginacijom i filterima.
        /// </summary>
        Task<AuditLogResponseDto> GetAuditLogsAsync(AuditLogFilterDto filter);

        /// <summary>
        /// Dohvata detalje jednog audit loga.
        /// </summary>
        Task<AuditLogDetailDto?> GetAuditLogDetailAsync(int id);

        /// <summary>
        /// Dohvata sve dostupne AuditActionType vrijednosti.
        /// </summary>
        Task<List<string>> GetActionTypesAsync();

        /// <summary>
        /// Dohvata korisnike koji imaju audit log zapise.
        /// </summary>
        Task<List<AuditLogUserDto>> GetAuditLogUsersAsync();
    }
}
