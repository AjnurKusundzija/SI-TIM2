using System;
using System.Collections.Generic;
using System.Text;

namespace TelecomSupportSystem.DAL.Entities.Enums
{
    public enum AssignmentType
    {
        AUTOMATIC = 1,
        MANUAL = 2,
        FORWARDED_TO_AGENT = 3,
        FORWARDED_TO_TECHNICIAN = 4
    }
}
