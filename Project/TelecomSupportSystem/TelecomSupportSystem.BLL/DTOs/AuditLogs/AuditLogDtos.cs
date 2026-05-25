namespace TelecomSupportSystem.BLL.DTOs.AuditLogs
{
    public class AuditLogListItemDto
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool HasDetails { get; set; }
    }

    public class AuditLogDetailDto : AuditLogListItemDto
    {
        public Dictionary<string, object?>? OldValue { get; set; }
        public Dictionary<string, object?>? NewValue { get; set; }
        public string? IpAddress { get; set; }
    }

    public class AuditLogResponseDto
    {
        public List<AuditLogListItemDto> Items { get; set; } = new List<AuditLogListItemDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class AuditLogFilterDto
    {
        public string? Search { get; set; }
        public string? ActionType { get; set; }
        public int? UserId { get; set; }
        public string? EntityType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class AuditLogUserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
