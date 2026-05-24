using System;
using System.Collections.Generic;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    // PB-52 / US-76: Katalog paketa koje firma nudi (admin-managed).
    // Razlikuje se od SubscriptionPackage (per-user pretplata iz PB-21) — ovo je apstraktni
    // katalog šablon koji administrator održava i koji se dodjeljuje klijentima preko
    // ClientSubscription entiteta.
    public class CatalogPackage
    {
        public int CatalogPackageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public PackageType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public PackageStatus Status { get; set; } = PackageStatus.ACTIVE;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public ICollection<ClientSubscription> Subscriptions { get; set; } = new List<ClientSubscription>();
    }
}
