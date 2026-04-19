using TelecomCustomerSupportSystem.Domain.Enums;

namespace TelecomCustomerSupportSystem.Application.DTOs.Notifications;

public class NotificationDto
{
    public int NotifikacijaId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Sadrzaj { get; set; } = string.Empty;
    public TipNotifikacije TipNotifikacije { get; set; }
    public DateTime DatumSlanja { get; set; }
    public bool Procitano { get; set; }
    public int KorisnikId { get; set; }
}