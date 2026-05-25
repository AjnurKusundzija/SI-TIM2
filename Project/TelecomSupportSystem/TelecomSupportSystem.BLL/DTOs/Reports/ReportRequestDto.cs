using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Reports
{
    public class ReportRequestDto
    {
        public ReportType ReportType { get; set; }
        public string Period { get; set; } = "month";
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
