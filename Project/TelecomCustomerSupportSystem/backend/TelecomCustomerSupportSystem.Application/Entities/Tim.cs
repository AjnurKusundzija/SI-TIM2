using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class Tim
{
    public int TimId { get; set; }
    public string NazivTima { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public TipTima TipTima { get; set; }
    public StatusTima StatusTima { get; set; }

    public ICollection<Korisnik> Clanovi { get; set; } = new List<Korisnik>();
    public ICollection<Tiket> Tiketi { get; set; } = new List<Tiket>();
    public ICollection<DodjelaTiketa> Dodjele { get; set; } = new List<DodjelaTiketa>();
}
