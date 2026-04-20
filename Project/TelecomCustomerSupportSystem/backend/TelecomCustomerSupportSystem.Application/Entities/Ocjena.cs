namespace TelecomCustomerSupportSystem.Application.Entities;

public class Ocjena
{
    public int OcjenaId { get; set; }
    public int VrijednostOcjene { get; set; }
    public string KomentarOcjene { get; set; } = string.Empty;
    public DateTime DatumOcjenjivanja { get; set; }
    public int KorisnikId { get; set; }
    public int TiketId { get; set; }

    public Korisnik Korisnik { get; set; } = null!;
    public Tiket Tiket { get; set; } = null!;
}
