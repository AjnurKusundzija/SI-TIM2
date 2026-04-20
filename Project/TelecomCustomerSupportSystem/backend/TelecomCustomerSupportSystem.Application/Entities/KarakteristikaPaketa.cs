namespace TelecomCustomerSupportSystem.Application.Entities;

public class KarakteristikaPaketa
{
    public int KarakteristikaId { get; set; }
    public string Naziv { get; set; } = string.Empty;
    public string Vrijednost { get; set; } = string.Empty;
    public string JedinicaMjere { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public int PaketId { get; set; }

    public PaketPretplata Paket { get; set; } = null!;
}
