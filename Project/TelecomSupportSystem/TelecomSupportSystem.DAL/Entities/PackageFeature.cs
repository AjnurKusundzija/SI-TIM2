using System;
using System.Collections.Generic;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities
{
    public class PackageFeature
    {
        public int FeatureId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PackageId { get; set; }

        public SubscriptionPackage Package { get; set; } = null!;
    }
}
