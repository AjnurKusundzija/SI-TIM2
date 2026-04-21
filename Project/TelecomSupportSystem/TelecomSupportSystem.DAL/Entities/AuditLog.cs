using System;
using System.Collections.Generic;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Table { get; set; } = string.Empty;
        public int? LogId { get; set; }
        public string? Value { get; set; }
        public int? UserId { get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public string? IpAdress { get; set; }
        public string? UserAgent { get; set; }
    }
}
