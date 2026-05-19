using System;
using System.Collections.Generic;

namespace TelecomSupportSystem.BLL.DTOs.Packages
{
    public class PackageDetailDto
    {
        public int PackageId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public string PackageStatus { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public string PackageDescription { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<PackageFeatureDto> Features { get; set; } = new List<PackageFeatureDto>();
    }
}
