using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class Tiket
{
    public int TiketId { get; set; }
    public string Naslov { get; set; } = string.Empty;
    public string Opis { get; set; } = string.Empty;
    public DateTime DatumKreiranja { get; set; }
    public DateTime? DatumZatvaranja { get; set; }
    public StatusTiketa Status { get; set; }
    public Prioritet Prioritet { get; set; }
    public KategorijaProblema KategorijaProblema { get; set; }
    public int KreatorId { get; set; }
    public int? TimId { get; set; }

    public Korisnik Kreator { get; set; } = null!;
    public Tim? Tim { get; set; }
    public ICollection<Komentar> Komentari { get; set; } = new List<Komentar>();
    public Ocjena? Ocjena { get; set; }
    public ICollection<DodjelaTiketa> Dodjele { get; set; } = new List<DodjelaTiketa>();
}
