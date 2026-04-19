using TelecomCustomerSupportSystem.Application.DTOs.Reports;

namespace TelecomCustomerSupportSystem.Application.Interfaces;

public interface IReportService
{
    Task<string?> GenerateAsync(ReportRequestDto request, CancellationToken cancellationToken = default);
}