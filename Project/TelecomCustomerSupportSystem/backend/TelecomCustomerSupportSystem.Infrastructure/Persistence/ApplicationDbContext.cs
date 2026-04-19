using Microsoft.EntityFrameworkCore;
using TelecomCustomerSupportSystem.Domain.Entities;

namespace TelecomCustomerSupportSystem.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for Telecom Customer Support System
/// Configured with proper relationships, indexing, and constraints
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Korisnik> Korisnici => Set<Korisnik>();
    public DbSet<Tim> Timovi => Set<Tim>();
    public DbSet<Tiket> Tiketi => Set<Tiket>();
    public DbSet<Komentar> Komentari => Set<Komentar>();
    public DbSet<Ocjena> Ocjene => Set<Ocjena>();
    public DbSet<PaketPretplata> PaketiPretplate => Set<PaketPretplata>();
    public DbSet<KarakteristikaPaketa> KarakteristikePaketa => Set<KarakteristikaPaketa>();
    public DbSet<DodjelaTiketa> DodjelaTiketa => Set<DodjelaTiketa>();
    public DbSet<Izvjestaj> Izvjestaji => Set<Izvjestaj>();
    public DbSet<Notifikacija> Notifikacije => Set<Notifikacija>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Korisnik configuration
        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.KorisnikId);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.KorisnickoIme).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LozinkaHash).IsRequired();
            entity.Property(e => e.Ime).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Prezime).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Adresa).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.KorisnickoIme).IsUnique();

            entity.HasMany(e => e.Tiketi)
                .WithOne(t => t.Kreator)
                .HasForeignKey(t => t.KreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Komentari)
                .WithOne(k => k.Autor)
                .HasForeignKey(k => k.AutorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.DodjeleTiketa)
                .WithOne(d => d.Korisnik)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Tim configuration
        modelBuilder.Entity<Tim>(entity =>
        {
            entity.HasKey(e => e.TimId);
            entity.Property(e => e.NazivTima).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Opis).HasMaxLength(500);

            entity.HasMany(e => e.Clanovi)
                .WithOne(k => k.Tim)
                .HasForeignKey(k => k.TimId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Tiket configuration
        modelBuilder.Entity<Tiket>(entity =>
        {
            entity.HasKey(e => e.TiketId);
            entity.Property(e => e.Naslov).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Opis).IsRequired();
            entity.Property(e => e.DatumKreiranja).IsRequired();

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Prioritet);
            entity.HasIndex(e => e.DatumKreiranja);
            entity.HasIndex(e => e.KreatorId);

            entity.HasMany(e => e.Komentari)
                .WithOne(k => k.Tiket)
                .HasForeignKey(k => k.TiketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Dodjele)
                .WithOne(d => d.Tiket)
                .HasForeignKey(d => d.TiketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Komentar configuration
        modelBuilder.Entity<Komentar>(entity =>
        {
            entity.HasKey(e => e.KomentarId);
            entity.Property(e => e.Sadrzaj).IsRequired();
            entity.Property(e => e.DatumVrijeme).IsRequired();

            entity.HasIndex(e => e.TiketId);
            entity.HasIndex(e => e.DatumVrijeme);
        });

        // Ocjena configuration
        modelBuilder.Entity<Ocjena>(entity =>
        {
            entity.HasKey(e => e.OcjenaId);
            entity.Property(e => e.VrijednostOcjene).IsRequired();
            entity.Property(e => e.DatumOcjenjivanja).IsRequired();

            entity.HasOne(e => e.Tiket)
                .WithOne(t => t.Ocjena)
                .HasForeignKey<Ocjena>(e => e.TiketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DodjelaTiketa configuration
        modelBuilder.Entity<DodjelaTiketa>(entity =>
        {
            entity.HasKey(e => e.DodjelaId);
            entity.Property(e => e.DatumDodjele).IsRequired();
            entity.Property(e => e.Napomena).HasMaxLength(500);

            entity.HasIndex(e => e.TiketId);
            entity.HasIndex(e => e.KorisnikId);
            entity.HasIndex(e => e.TimId);
        });

        // Notifikacija configuration
        modelBuilder.Entity<Notifikacija>(entity =>
        {
            entity.HasKey(e => e.NotifikacijaId);
            entity.Property(e => e.Naslov).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Sadrzaj).IsRequired();
            entity.Property(e => e.DatumSlanja).IsRequired();

            entity.HasIndex(e => e.KorisnikId);
            entity.HasIndex(e => e.DatumSlanja);
            entity.HasIndex(e => e.Procitano);
        });

        // PaketPretplata configuration
        modelBuilder.Entity<PaketPretplata>(entity =>
        {
            entity.HasKey(e => e.PaketId);
            entity.Property(e => e.NazivPaketa).IsRequired().HasMaxLength(100);
            entity.Property(e => e.OpisPaketa).HasMaxLength(500);

            entity.HasIndex(e => e.KorisnikId);
        });

        // KarakteristikaPaketa configuration
        modelBuilder.Entity<KarakteristikaPaketa>(entity =>
        {
            entity.HasKey(e => e.KarakteristikaId);
            entity.Property(e => e.Naziv).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Vrijednost).IsRequired().HasMaxLength(255);

            entity.HasIndex(e => e.PaketId);
        });

        // Izvjestaj configuration
        modelBuilder.Entity<Izvjestaj>(entity =>
        {
            entity.HasKey(e => e.IzvjestajId);
            entity.Property(e => e.Naziv).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DatumGenerisanja).IsRequired();

            entity.HasIndex(e => e.TipIzvjestaja);
            entity.HasIndex(e => e.DatumGenerisanja);
        });

        // AuditLog configuration (for security/compliance)
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);
            entity.Property(e => e.Akcija).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Tabela).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Vrijednost).HasMaxLength(4000);
            entity.Property(e => e.DatumAkcije).IsRequired();

            entity.HasIndex(e => e.DatumAkcije);
            entity.HasIndex(e => e.KorisnikId);
            entity.HasIndex(e => e.Tabela);

            entity.Property(e => e.Vrijednost).HasColumnType("nvarchar(max)");
        });
    }
}