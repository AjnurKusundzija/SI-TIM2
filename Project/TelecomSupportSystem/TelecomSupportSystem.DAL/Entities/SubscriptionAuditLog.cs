using System;

namespace TelecomSupportSystem.DAL.Entities
{
    // PB-52 / US-77 AC: Evidentira svaku promjenu pretplate (kreiranje, deaktivaciju).
    public class SubscriptionAuditLog
    {
        public int SubscriptionAuditLogId { get; set; }
        public int UserId { get; set; }
        public int AdminId { get; set; }
        public int CatalogPackageId { get; set; }
        public int? SubscriptionId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
