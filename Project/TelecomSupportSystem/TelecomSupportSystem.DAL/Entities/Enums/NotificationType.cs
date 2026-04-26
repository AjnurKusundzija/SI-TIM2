using System;
using System.Collections.Generic;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities.Enums
{
    public enum NotificationType
    {
        TICKET_ASSIGNED = 1,
        TICKET_FORWARDED = 2,
        STATUS_CHANGED = 3,
        TICKET_RESPONSE = 4,
        TICKET_CLOSED = 5
    }
}
