using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class DodjelaTiketa
{
    public int DodjelaId { get; set; }
    public DateTime DatumDodjele { get; set; }
    public TipDodjele TipDodjele { get; set; }
    public string Napomena { get; set; } = string.Empty;
    public int TiketId { get; set; }
    public int KorisnikId { get; set; }
    public int TimId { get; set; }

    public Tiket Tiket { get; set; } = null!;
    public Korisnik Korisnik { get; set; } = null!;
    public Tim Tim { get; set; } = null!;
}
