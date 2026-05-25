using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketUser> TicketUsers => Set<TicketUser>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<SubscriptionPackage> SubscriptionPackages => Set<SubscriptionPackage>();
    public DbSet<PackageFeature> PackageFeatures => Set<PackageFeature>();
    public DbSet<CatalogPackage> CatalogPackages => Set<CatalogPackage>();
    public DbSet<ClientSubscription> ClientSubscriptions => Set<ClientSubscription>();
    public DbSet<SubscriptionAuditLog> SubscriptionAuditLogs => Set<SubscriptionAuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Faq> Faqs => Set<Faq>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Location).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasMany(e => e.Tickets)
                .WithOne(t => t.Creator)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Comments)
                .WithOne(c => c.Author)
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Ratings)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Packages)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId);

            entity.Property(e => e.TeamName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasMany(e => e.Members)
                .WithOne(u => u.Team)
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Tickets)
                .WithOne(t => t.Team)
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.CreatorId);

            entity.HasMany(e => e.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Attachments)
                .WithOne(a => a.Ticket)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Restrict); // Ovdje je spriječeno ciklično kaskadiranje

            entity.HasOne(e => e.Rating)
                .WithOne(r => r.Ticket)
                .HasForeignKey<Rating>(r => r.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId);

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.DateTime).IsRequired();

            entity.HasIndex(e => e.TicketId);
            entity.HasIndex(e => e.DateTime);

            entity.HasMany(e => e.Attachments)
                .WithOne(a => a.Comment)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId);

            entity.Property(e => e.FileName).IsRequired().HasMaxLength(260);
            entity.Property(e => e.StoredFileName).IsRequired().HasMaxLength(260);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Size).IsRequired();
            entity.Property(e => e.UploadedAt).IsRequired();

            entity.HasIndex(e => e.TicketId);
            entity.HasIndex(e => e.CommentId);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.Attachments)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Restrict); // Sinkronizirano s Ticket konfiguracijom

            entity.HasOne(e => e.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(e => e.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId);

            entity.Property(e => e.RatingValue).IsRequired();
            entity.Property(e => e.RatingDate).IsRequired();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);

            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.SentDate).IsRequired();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.SentDate);
            entity.HasIndex(e => e.IsRead);
        });

        modelBuilder.Entity<SubscriptionPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId);

            entity.Property(e => e.PackageName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PackageDescription).HasMaxLength(500);
            entity.Property(e => e.MonthlyPrice).HasPrecision(18, 2);

            entity.HasIndex(e => e.UserId);

            entity.HasMany(e => e.Features)
                .WithOne(f => f.Package)
                .HasForeignKey(f => f.PackageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PackageFeature>(entity =>
        {
            entity.HasKey(e => e.FeatureId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.PackageId);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.GeneratedDate).IsRequired();

            entity.HasIndex(e => e.ReportType);
            entity.HasIndex(e => e.GeneratedDate);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);

            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Table).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ActionDate).IsRequired();

            entity.HasIndex(e => e.ActionDate);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Table);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.RefreshTokenId);

            entity.Property(e => e.Token).IsRequired().HasMaxLength(512);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Faq>(entity =>
        {
            entity.HasKey(e => e.FaqId);

            entity.Property(e => e.Question).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Answer).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.SortOrder).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();

            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.SortOrder);
        });

        // PB-52 / US-76
        modelBuilder.Entity<CatalogPackage>(entity =>
        {
            entity.HasKey(e => e.CatalogPackageId);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.CreatedDate).IsRequired();

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Type);

            entity.HasMany(e => e.Subscriptions)
                .WithOne(s => s.CatalogPackage)
                .HasForeignKey(s => s.CatalogPackageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // PB-52 / US-77
        modelBuilder.Entity<ClientSubscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId);

            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CatalogPackageId);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PB-52 / US-77 — audit log za promjene pretplata
        modelBuilder.Entity<SubscriptionAuditLog>(entity =>
        {
            entity.HasKey(e => e.SubscriptionAuditLogId);

            entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Timestamp).IsRequired();

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.AdminId);
            entity.HasIndex(e => e.CatalogPackageId);
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<TicketUser>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);

            entity.Property(e => e.AssignmentDate).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasIndex(e => e.TicketId);
            entity.HasIndex(e => e.UserId);

            entity.HasOne(e => e.Ticket)
                .WithMany(t => t.Assignments)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany(u => u.TicketAssignments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team)
                .WithMany(t => t.Assignments)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}