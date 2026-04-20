using TelecomCustomerSupportSystem.Application.Enums;

namespace TelecomCustomerSupportSystem.Application.Entities;

public class Korisnik
{
    public int KorisnikId { get; set; }
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefon { get; set; } = string.Empty;
    public string Adresa { get; set; } = string.Empty;
    public string KorisnickoIme { get; set; } = string.Empty;
    public string LozinkaHash { get; set; } = string.Empty;
    public StatusNaloga StatusNaloga { get; set; }
    public Uloga Uloga { get; set; }
    public StatusRaspolozivosti? StatusRaspolozivosti { get; set; }
    public int? TimId { get; set; }

    public Tim? Tim { get; set; }
    public ICollection<Tiket> Tiketi { get; set; } = new List<Tiket>();
    public ICollection<Komentar> Komentari { get; set; } = new List<Komentar>();
    public ICollection<Ocjena> Ocjene { get; set; } = new List<Ocjena>();
    public ICollection<Notifikacija> Notifikacije { get; set; } = new List<Notifikacija>();
    public ICollection<PaketPretplata> Paketi { get; set; } = new List<PaketPretplata>();
    public ICollection<DodjelaTiketa> DodjeleTiketa { get; set; } = new List<DodjelaTiketa>();
}
