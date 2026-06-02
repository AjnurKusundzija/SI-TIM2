namespace TelecomSupportSystem.DAL.Entities.Enums
{
    public enum AuditActionType
    {
        USER_LOGIN = 1,
        USER_LOGOUT = 2,
        USER_LOGIN_FAILED = 3,
        USER_CREATED = 4,
        USER_UPDATED = 5,
        USER_DEACTIVATED = 6,
        USER_REACTIVATED = 7,
        TICKET_CREATED = 8,
        TICKET_CLOSED = 9,
        TICKET_CLOSURE_REQUESTED = 10,
        TICKET_STATUS_CHANGED = 11,
        TICKET_FORWARDED = 12,
        TICKET_PRIORITY_CHANGED = 13,
        PACKAGE_CREATED = 14,
        PACKAGE_UPDATED = 15,
        PACKAGE_DEACTIVATED = 16,
        SUBSCRIPTION_ASSIGNED = 17,
        SUBSCRIPTION_DEACTIVATED = 18,
        AGENT_REASSIGNED = 19
    }
}
