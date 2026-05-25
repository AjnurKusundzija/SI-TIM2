using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? OldValue { get; set; }   // JSON string
        public string? NewValue { get; set; }   // JSON string
        public string? IpAddress { get; set; }
    }
}
