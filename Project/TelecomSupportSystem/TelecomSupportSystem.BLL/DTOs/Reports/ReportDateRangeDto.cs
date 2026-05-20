namespace TelecomSupportSystem.BLL.DTOs.Reports
{
    public class ReportDateRangeDto
    {
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
    }
}
