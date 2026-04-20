namespace TelecomCustomerSupportSystem.Application.Entities;

public class Komentar
{
    public int KomentarId { get; set; }
    public string Sadrzaj { get; set; } = string.Empty;
    public DateTime DatumVrijeme { get; set; }
    public bool JeInterni { get; set; }
    public int AutorId { get; set; }
    public int TiketId { get; set; }

    public Korisnik Autor { get; set; } = null!;
    public Tiket Tiket { get; set; } = null!;
}
