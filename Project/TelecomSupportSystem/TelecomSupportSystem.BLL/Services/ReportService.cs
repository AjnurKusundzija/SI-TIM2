using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.Helpers;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class ReportService : IReportService
    {
        private const int StaleTicketDays = 7;
        private const int TopItemsLimit = 5;

        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(string period, DateTime? from, DateTime? to)
        {
            var (periodFrom, periodTo, label) = ReportPeriodHelper.Resolve(period, from, to);
            var tickets = await _reportRepository.GetTicketsCreatedInPeriodAsync(periodFrom, periodTo);
            var ticketList = tickets.ToList();

            var statusBreakdown = BuildStatusBreakdown(ticketList);
            var roleCounts = await _reportRepository.GetActiveUserCountsByRoleAsync();
            var workload = await _reportRepository.GetAgentResolvedCountsAsync(periodFrom, periodTo);
            var staleThreshold = DateTime.UtcNow.AddDays(-StaleTicketDays);
            var firstResponseReport = FirstResponseReportHelper.Build(ticketList, period, periodFrom, periodTo);

            return new AdminDashboardDto
            {
                Period = new ReportDateRangeDto
                {
                    PeriodFrom = periodFrom,
                    PeriodTo = periodTo,
                    PeriodLabel = label,
                },
                TotalTicketsInPeriod = ticketList.Count,
                StatusBreakdown = statusBreakdown,
                AvgFirstResponseMinutes = firstResponseReport.AvgFirstResponseMinutes,
                FirstResponseBucketGranularityLabel = firstResponseReport.BucketGranularityLabel,
                FirstResponseByPeriod = firstResponseReport.Buckets,
                AvgResolutionHours = TicketMetricsHelper.CalculateAvgResolutionHours(ticketList),
                ClosedInPeriodCount = await _reportRepository.GetClosedInPeriodCountAsync(periodFrom, periodTo),
                AvgRating = TicketMetricsHelper.CalculateAvgRating(ticketList),
                TopProblemTypes = BuildProblemTypeSummary(ticketList),
                TopAgentWorkload = MapWorkload(workload.Take(TopItemsLimit)),
                ActiveUsersByRole = new UserRoleCountsDto
                {
                    Clients = roleCounts.Clients,
                    Agents = roleCounts.Agents,
                    Technicians = roleCounts.Technicians,
                    Administrators = roleCounts.Administrators,
                },
                OpenTicketsCount = await _reportRepository.GetOpenTicketsCountAsync(),
                ClosureRequestedCount = await _reportRepository.GetClosureRequestedCountAsync(),
                UnassignedOpenCount = await _reportRepository.GetUnassignedOpenTicketsCountAsync(),
                StaleTicketsCount = await _reportRepository.GetStaleTicketsCountAsync(staleThreshold),
            };
        }

        public async Task<ReportResultDto> GenerateReportAsync(
            ReportType reportType,
            string period,
            DateTime? from,
            DateTime? to)
        {
            var (periodFrom, periodTo, label) = ReportPeriodHelper.Resolve(period, from, to);
            var tickets = await _reportRepository.GetTicketsCreatedInPeriodAsync(periodFrom, periodTo);
            var ticketList = tickets.ToList();
            var largePeriod = ReportPeriodHelper.IsLargePeriod(periodFrom, periodTo);

            var result = new ReportResultDto
            {
                ReportType = reportType.ToString(),
                Period = new ReportDateRangeDto
                {
                    PeriodFrom = periodFrom,
                    PeriodTo = periodTo,
                    PeriodLabel = label,
                },
                ShowLargePeriodWarning = reportType == ReportType.TICKET_STATUS && largePeriod,
            };

            if (ticketList.Count == 0 && reportType != ReportType.TEAM_WORKLOAD)
            {
                result.HasData = false;
                result.Message = "Nema podataka za odabrani period.";
                return result;
            }

            result.HasData = true;
            result.Data = reportType switch
            {
                ReportType.TICKET_COUNT => new TicketCountReportDto { TotalCount = ticketList.Count },
                ReportType.TICKET_STATUS => new TicketStatusReportDto { Items = BuildStatusBreakdown(ticketList) },
                ReportType.PROBLEM_TYPE => new ProblemTypeReportDto { Items = BuildProblemTypeSummary(ticketList) },
                ReportType.TEAM_WORKLOAD => await BuildTeamWorkloadReportAsync(periodFrom, periodTo),
                ReportType.USER_RATINGS => BuildUserRatingsReport(ticketList),
                ReportType.FIRST_RESPONSE => BuildFirstResponseReport(ticketList, period, periodFrom, periodTo),
                _ => throw new ArgumentException("Nepoznat tip izvještaja."),
            };

            if (result.Data is FirstResponseReportDto fr && fr.TotalTicketsCount == 0)
            {
                result.HasData = false;
                result.Message = "Nema podataka za odabrani period.";
                result.Data = fr;
            }
            else if (result.Data is FirstResponseReportDto frNoResponse && frNoResponse.TicketsWithResponseCount == 0)
            {
                result.HasData = true;
                result.Message = "Nema tiketa s prvim odgovorom u odabranom periodu.";
            }

            if (result.Data is TeamWorkloadReportDto wl && wl.Items.Count == 0)
            {
                result.HasData = false;
                result.Message = "Nema podataka za odabrani period.";
                result.Data = wl;
            }

            return result;
        }

        private static IReadOnlyList<StatusCountDto> BuildStatusBreakdown(List<DAL.Entities.Ticket> tickets)
        {
            if (tickets.Count == 0)
                return Array.Empty<StatusCountDto>();

            return Enum.GetValues<TicketStatus>()
                .Select(status =>
                {
                    var count = tickets.Count(t => t.Status == status);
                    return new StatusCountDto
                    {
                        Status = status.ToString(),
                        Count = count,
                        Percentage = Math.Round(count * 100.0 / tickets.Count, 1),
                    };
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToList();
        }

        private static IReadOnlyList<NamedCountDto> BuildProblemTypeSummary(List<DAL.Entities.Ticket> tickets) =>
            tickets
                .GroupBy(t => t.ProblemCategory)
                .Select(g => new NamedCountDto
                {
                    Name = g.Key.ToString(),
                    Count = g.Count(),
                })
                .OrderByDescending(x => x.Count)
                .Take(TopItemsLimit)
                .ToList();

        private static IReadOnlyList<AgentWorkloadDto> MapWorkload(IEnumerable<AgentResolveRow> rows) =>
            rows.Select(r => new AgentWorkloadDto
            {
                UserId = r.UserId,
                FullName = $"{r.FirstName} {r.LastName}".Trim(),
                Role = r.Role.ToString(),
                ResolvedCount = r.ResolvedCount,
            }).ToList();

        private async Task<TeamWorkloadReportDto> BuildTeamWorkloadReportAsync(DateTime from, DateTime to)
        {
            var rows = await _reportRepository.GetAgentResolvedCountsAsync(from, to);
            return new TeamWorkloadReportDto { Items = MapWorkload(rows) };
        }

        private static FirstResponseReportDto BuildFirstResponseReport(
            List<DAL.Entities.Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo) =>
            FirstResponseReportHelper.Build(tickets, period, periodFrom, periodTo);

        private static UserRatingsReportDto BuildUserRatingsReport(List<DAL.Entities.Ticket> tickets)
        {
            var rated = tickets.Where(t => t.Rating != null).ToList();
            if (rated.Count == 0)
                return new UserRatingsReportDto();

            var distribution = rated
                .GroupBy(t => t.Rating!.RatingValue)
                .Select(g => new RatingDistributionDto { Stars = g.Key, Count = g.Count() })
                .OrderBy(d => d.Stars)
                .ToList();

            return new UserRatingsReportDto
            {
                AverageRating = rated.Average(t => t.Rating!.RatingValue),
                RatedTicketsCount = rated.Count,
                Distribution = distribution,
            };
        }
    }
}
