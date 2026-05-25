using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IReportService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync(string period, DateTime? from, DateTime? to);
        Task<ReportResultDto> GenerateReportAsync(ReportType reportType, string period, DateTime? from, DateTime? to);
    }
}
