using Microsoft.EntityFrameworkCore;
using TelecomCustomerSupportSystem.Domain.Entities;

namespace TelecomCustomerSupportSystem.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Korisnik> Korisnici => Set<Korisnik>();
    public DbSet<Tim> Timovi => Set<Tim>();
    public DbSet<Tiket> Tiketi => Set<Tiket>();
    public DbSet<Komentar> Komentari => Set<Komentar>();
    public DbSet<Ocjena> Ocjene => Set<Ocjena>();
    public DbSet<PaketPretplata> PaketiPretplate => Set<PaketPretplata>();
    public DbSet<KarakteristikaPaketa> KarakteristikePaketa => Set<KarakteristikaPaketa>();
    public DbSet<DodjelaTiketa> DodjeleTiketa => Set<DodjelaTiketa>();
    public DbSet<Izvjestaj> Izvjestaji => Set<Izvjestaj>();
    public DbSet<Notifikacija> Notifikacije => Set<Notifikacija>();
}