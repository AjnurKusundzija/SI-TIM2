namespace TelecomCustomerSupportSystem.Application.Entities;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public string Akcija { get; set; } = string.Empty;
    public string Tabela { get; set; } = string.Empty;
    public int? ZapisId { get; set; }
    public string? Vrijednost { get; set; }
    public int? KorisnikId { get; set; }
    public DateTime DatumAkcije { get; set; } = DateTime.UtcNow;
    public string? IpAdresa { get; set; }
    public string? UserAgent { get; set; }
}
