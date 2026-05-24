using System;

namespace TelecomSupportSystem.BLL.DTOs.Subscriptions
{
    public class ClientSubscriptionDto
    {
        public int SubscriptionId { get; set; }
        public int CatalogPackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PackageDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
