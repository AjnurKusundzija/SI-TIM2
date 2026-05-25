using System;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    // PB-52 / US-77: Veza klijent ↔ katalog paket (jedna pretplata po redu).
    public class ClientSubscription
    {
        public int SubscriptionId { get; set; }
        public int CatalogPackageId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public PackageStatus Status { get; set; } = PackageStatus.ACTIVE;

        public CatalogPackage CatalogPackage { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
