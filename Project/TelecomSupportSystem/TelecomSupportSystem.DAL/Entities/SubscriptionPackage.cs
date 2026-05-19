using System;
using System.Collections.Generic;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class SubscriptionPackage
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public PackageType PackageType { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string PackageDescription { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public User User { get; set; } = null!;
        public ICollection<PackageFeature> Features { get; set; } = new List<PackageFeature>();
    }
}
