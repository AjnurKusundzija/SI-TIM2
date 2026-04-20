using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class Notifikacija
{
    public int NotifikacijaId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Sadrzaj { get; set; } = string.Empty;
    public TipNotifikacije TipNotifikacije { get; set; }
    public DateTime DatumSlanja { get; set; }
    public bool Procitano { get; set; }
    public int KorisnikId { get; set; }

    public Korisnik Korisnik { get; set; } = null!;
}
