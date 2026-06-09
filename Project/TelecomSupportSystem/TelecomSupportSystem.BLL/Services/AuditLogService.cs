using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.AuditLogs;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.Services
{
    /// <summary>
    /// Servis za logiranje audit akcija.
    /// Ne sprema osjetljiva polja koja sadrže: password, secret, token, hash (case-insensitive).
    /// </summary>
    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AuditLogService> _logger;

        // Osjetljive polje — nikada se ne čuvaju
        private static readonly string[] SensitiveFields = { "password", "secret", "token", "hash" };

        public AuditLogService(ApplicationDbContext dbContext, ILogger<AuditLogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task LogAsync(
            AuditActionType actionType,
            string entityType,
            string? entityId,
            string description,
            int? userId = null,
            object? oldValue = null,
            object? newValue = null,
            string? ipAddress = null
        )
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    ActionType = actionType.ToString(),
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = description,
                    OldValue = SerializeValue(oldValue),
                    NewValue = SerializeValue(newValue),
                    IpAddress = ipAddress
                };

                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri logiranju audit akcije. ActionType: {ActionType}, EntityType: {EntityType}", actionType, entityType);
                // Ne propagiraj iznimku prema pozivaocu — audit log mora biti non-blocking
            }
        }

        public async Task<AuditLogResponseDto> GetAuditLogsAsync(AuditLogFilterDto filter)
        {
            try
            {
                var query = _dbContext.AuditLogs.AsQueryable();

                // Filter po ActionType
                if (!string.IsNullOrWhiteSpace(filter.ActionType))
                {
                    query = query.Where(a => a.ActionType == filter.ActionType);
                }

                // Filter po UserId
                if (filter.UserId.HasValue && filter.UserId.Value > 0)
                {
                    query = query.Where(a => a.UserId == filter.UserId.Value);
                }

                if (!string.IsNullOrWhiteSpace(filter.EntityType))
                {
                    query = query.Where(a => a.EntityType == filter.EntityType);
                }

                // Filter po Description (LIKE)
                if (!string.IsNullOrWhiteSpace(filter.Search))
                {
                    var searchTerm = filter.Search.ToLower();
                    query = query.Where(a => a.Description.ToLower().Contains(searchTerm));
                }

                // Filter po datumskom rasponu
                if (filter.DateFrom.HasValue)
                {
                    query = query.Where(a => a.Timestamp >= filter.DateFrom.Value);
                }

                if (filter.DateTo.HasValue)
                {
                    var dateTo = filter.DateTo.Value.AddDays(1); // Uključi cijeli dan
                    query = query.Where(a => a.Timestamp < dateTo);
                }

                var totalCount = await query.CountAsync();

                // Sortiraj po vremenu opadajuće
                var items = await query
                    .OrderByDescending(a => a.Timestamp)
                    .Skip((filter.Page - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .Include(a => a.User)
                    .Select(a => new AuditLogListItemDto
                    {
                        Id = a.Id,
                        Timestamp = a.Timestamp,
                        UserId = a.UserId,
                        UserFullName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : null,
                        UserEmail = a.User != null ? a.User.Email : null,
                        UserRole = a.User != null ? a.User.Role.ToString() : null,
                        ActionType = a.ActionType,
                        EntityType = a.EntityType,
                        EntityId = a.EntityId,
                        Description = a.Description,
                        HasDetails = !string.IsNullOrWhiteSpace(a.OldValue) || !string.IsNullOrWhiteSpace(a.NewValue)
                    })
                    .ToListAsync();

                var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

                return new AuditLogResponseDto
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = filter.Page,
                    PageSize = filter.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri dohvatanju audit logova");
                throw;
            }
        }

        public async Task<AuditLogDetailDto?> GetAuditLogDetailAsync(int id)
        {
            try
            {
                var auditLog = await _dbContext.AuditLogs
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (auditLog == null)
                    return null;

                return new AuditLogDetailDto
                {
                    Id = auditLog.Id,
                    Timestamp = auditLog.Timestamp,
                    UserId = auditLog.UserId,
                    UserFullName = auditLog.User != null ? $"{auditLog.User.FirstName} {auditLog.User.LastName}" : null,
                    UserEmail = auditLog.User != null ? auditLog.User.Email : null,
                    UserRole = auditLog.User != null ? auditLog.User.Role.ToString() : null,
                    ActionType = auditLog.ActionType,
                    EntityType = auditLog.EntityType,
                    EntityId = auditLog.EntityId,
                    Description = auditLog.Description,
                    HasDetails = !string.IsNullOrWhiteSpace(auditLog.OldValue) || !string.IsNullOrWhiteSpace(auditLog.NewValue),
                    OldValue = DeserializeValue(auditLog.OldValue),
                    NewValue = DeserializeValue(auditLog.NewValue),
                    IpAddress = auditLog.IpAddress
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri dohvatanju detalja audit loga. Id: {Id}", id);
                throw;
            }
        }

        public async Task<List<string>> GetActionTypesAsync()
        {
            try
            {
                return await System.Threading.Tasks.Task.FromResult(
                    Enum.GetValues(typeof(AuditActionType))
                        .Cast<AuditActionType>()
                        .Select(e => e.ToString())
                        .ToList()
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri dohvatanju tipova akcija");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(AuditActionType actionType, string entityId)
        {
            var actionTypeStr = actionType.ToString();
            return await _dbContext.AuditLogs.AnyAsync(a =>
                a.ActionType == actionTypeStr && a.EntityId == entityId);
        }

        public async Task<List<AuditLogUserDto>> GetAuditLogUsersAsync()
        {
            try
            {
                return await _dbContext.AuditLogs
                    .Where(a => a.User != null)
                    .GroupBy(a => a.UserId)
                    .Select(g => new AuditLogUserDto
                    {
                        Id = g.Key ?? 0,
                        FullName = g.First().User!.FirstName + " " + g.First().User!.LastName,
                        Email = g.First().User!.Email
                    })
                    .OrderBy(u => u.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška pri dohvatanju korisnika sa audit logovima");
                throw;
            }
        }

        /// <summary>
        /// Serijalizuje vrijednost u JSON string, ispuštajući osjetljiva polja.
        /// </summary>
        private string? SerializeValue(object? value)
        {
            if (value == null)
                return null;

            try
            {
                if (value is string strValue)
                {
                    return strValue;
                }

                var node = JsonSerializer.SerializeToNode(value);
                RemoveSensitiveFields(node);
                return node?.ToJsonString();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Greška pri serijalizaciji vrijednosti za audit log");
                return null;
            }
        }

        /// <summary>
        /// Deserijalizuje JSON string u Dictionary.
        /// </summary>
        private Dictionary<string, object?>? DeserializeValue(string? jsonValue)
        {
            if (string.IsNullOrWhiteSpace(jsonValue))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonValue);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Greška pri deserijalizaciji vrijednosti iz audit loga");
                return null;
            }
        }

        private static void RemoveSensitiveFields(JsonNode? node)
        {
            if (node is JsonObject obj)
            {
                var keysToRemove = obj
                    .Select(kvp => kvp.Key)
                    .Where(key => SensitiveFields.Any(sf => key.Contains(sf, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                foreach (var key in keysToRemove)
                    obj.Remove(key);

                foreach (var child in obj.Select(kvp => kvp.Value).ToList())
                    RemoveSensitiveFields(child);
            }
            else if (node is JsonArray array)
            {
                foreach (var child in array)
                    RemoveSensitiveFields(child);
            }
        }
    }
}
