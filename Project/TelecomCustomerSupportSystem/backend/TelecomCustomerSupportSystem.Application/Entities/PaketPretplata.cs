using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class PaketPretplata
{
    public int PaketId { get; set; }
    public string NazivPaketa { get; set; } = string.Empty;
    public TipPaketa TipPaketa { get; set; }
    public StatusPaketa StatusPaketa { get; set; }
    public decimal MjesecnaCijena { get; set; }
    public string OpisPaketa { get; set; } = string.Empty;
    public int KorisnikId { get; set; }

    public Korisnik Korisnik { get; set; } = null!;
    public ICollection<KarakteristikaPaketa> Karakteristike { get; set; } = new List<KarakteristikaPaketa>();
}
