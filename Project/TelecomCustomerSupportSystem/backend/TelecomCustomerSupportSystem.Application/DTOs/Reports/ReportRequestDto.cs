using TelecomCustomerSupportSystem.Domain.Enums;

namespace TelecomCustomerSupportSystem.Application.DTOs.Reports;

public class ReportRequestDto
{
    public TipIzvjestaja TipIzvjestaja { get; set; }
    public DateTime PeriodOd { get; set; }
    public DateTime PeriodDo { get; set; }
}