namespace TelecomCustomerSupportSystem.Domain.Entities;

/// <summary>
/// AuditLog entity for tracking all user actions and system changes for security and compliance
/// </summary>
public class AuditLog
{
    public int AuditLogId { get; set; }
    public string Akcija { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, LOGIN, etc.
    public string Tabela { get; set; } = string.Empty; // Table name affected
    public int? ZapisId { get; set; } // ID of record affected
    public string? Vrijednost { get; set; } // JSON serialized old/new values
    public int? KorisnikId { get; set; } // Who performed the action
    public DateTime DatumAkcije { get; set; } = DateTime.UtcNow;
    public string? IpAdresa { get; set; }
    public string? UserAgent { get; set; }
}
