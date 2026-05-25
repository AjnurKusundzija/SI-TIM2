using System;
using System.Collections.Generic;

namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class PackageSummaryDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PackageStatus { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public string PackageDescription { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> IncludedServices { get; set; } = new List<string>();
        public DateTime? StartDate { get; set; }
    }
}
