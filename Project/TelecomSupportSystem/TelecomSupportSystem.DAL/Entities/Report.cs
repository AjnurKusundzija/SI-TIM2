using System;
using System.Collections.Generic;
using System.Text;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Entities
{
    public class Report
    {
        public int ReportId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public DateTime GeneratedDate { get; set; }
    }
}
