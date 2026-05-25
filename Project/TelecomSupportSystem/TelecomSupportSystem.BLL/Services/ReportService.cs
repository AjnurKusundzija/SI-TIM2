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
                ReportType.TICKET_COUNT    => BuildTicketCountReport(ticketList, period, periodFrom, periodTo),
                ReportType.TICKET_STATUS   => new TicketStatusReportDto { Items = BuildStatusBreakdown(ticketList) },
                ReportType.PROBLEM_TYPE    => new ProblemTypeReportDto { Items = BuildProblemTypeSummary(ticketList) },
                ReportType.TEAM_WORKLOAD   => await BuildTeamWorkloadReportAsync(period, periodFrom, periodTo),
                ReportType.USER_RATINGS    => BuildUserRatingsReport(ticketList, period, periodFrom, periodTo),
                ReportType.FIRST_RESPONSE  => BuildFirstResponseReport(ticketList, period, periodFrom, periodTo),
                ReportType.AVG_RESOLUTION  => BuildAvgResolutionReport(ticketList, period, periodFrom, periodTo),
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

            if (result.Data is AvgResolutionReportDto ar && ar.ClosedTicketsCount == 0)
            {
                result.HasData = true;
                result.Message = "Nema zatvorenih tiketa u odabranom periodu.";
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

        private async Task<TeamWorkloadReportDto> BuildTeamWorkloadReportAsync(
            string period,
            DateTime periodFrom,
            DateTime periodTo)
        {
            var totals = await _reportRepository.GetAgentResolvedCountsAsync(periodFrom, periodTo);
            var details = await _reportRepository.GetAgentResolvedDetailsAsync(periodFrom, periodTo) ?? [];

            var agentTotalsList = MapWorkload(totals);
            var agentNames = agentTotalsList.Select(a => a.FullName).ToList();
            var agentIdToName = totals
                .ToDictionary(r => r.UserId, r => $"{r.FirstName} {r.LastName}".Trim());

            var (granularity, granularityLabel) = FirstResponseReportHelper.ResolveGranularity(period, periodFrom, periodTo);
            var ranges = FirstResponseReportHelper.GenerateRanges(periodFrom, periodTo, granularity);

            var periodRows = ranges.Select(r =>
            {
                var inBucket = details
                    .Where(d => d.ClosedDate >= r.Start && d.ClosedDate < r.End)
                    .ToList();
                var counts = agentNames
                    .Select(name => inBucket.Count(d =>
                        agentIdToName.TryGetValue(d.UserId, out var n) && n == name))
                    .ToList();
                return new WorkloadPeriodRowDto { Label = r.Label, Counts = counts };
            }).ToList();

            return new TeamWorkloadReportDto
            {
                Items = agentTotalsList,
                BucketGranularityLabel = granularityLabel,
                AgentNames = agentNames,
                PeriodRows = periodRows,
            };
        }

        private static FirstResponseReportDto BuildFirstResponseReport(
            List<DAL.Entities.Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo) =>
            FirstResponseReportHelper.Build(tickets, period, periodFrom, periodTo);

        private static TicketCountReportDto BuildTicketCountReport(
            List<DAL.Entities.Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo)
        {
            var (granularity, granularityLabel) = FirstResponseReportHelper.ResolveGranularity(period, periodFrom, periodTo);
            var ranges = FirstResponseReportHelper.GenerateRanges(periodFrom, periodTo, granularity);
            var buckets = ranges
                .Select(r => new CountBucketDto
                {
                    Label = r.Label,
                    TicketCount = tickets.Count(t => t.CreatedDate >= r.Start && t.CreatedDate < r.End),
                })
                .ToList();

            return new TicketCountReportDto
            {
                TotalCount = tickets.Count,
                BucketGranularityLabel = granularityLabel,
                Buckets = buckets,
            };
        }

        private static AvgResolutionReportDto BuildAvgResolutionReport(
            List<DAL.Entities.Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo)
        {
            var closed = tickets.Where(t => t.Status == DAL.Entities.Enums.TicketStatus.CLOSED && t.ClosedDate.HasValue).ToList();

            var (granularity, granularityLabel) = FirstResponseReportHelper.ResolveGranularity(period, periodFrom, periodTo);
            var ranges = FirstResponseReportHelper.GenerateRanges(periodFrom, periodTo, granularity);

            var buckets = ranges.Select(r =>
            {
                var inBucket = tickets.Where(t => t.CreatedDate >= r.Start && t.CreatedDate < r.End).ToList();
                var closedInBucket = inBucket
                    .Where(t => t.Status == DAL.Entities.Enums.TicketStatus.CLOSED && t.ClosedDate.HasValue)
                    .ToList();
                var avgHours = closedInBucket.Count > 0
                    ? closedInBucket.Average(t => (t.ClosedDate!.Value - t.CreatedDate).TotalHours)
                    : (double?)null;

                return new ResolutionBucketDto
                {
                    Label = r.Label,
                    TicketCount = inBucket.Count,
                    ClosedCount = closedInBucket.Count,
                    AvgResolutionHours = avgHours.HasValue ? Math.Round(avgHours.Value, 2) : null,
                };
            }).ToList();

            return new AvgResolutionReportDto
            {
                AvgResolutionHours = TicketMetricsHelper.CalculateAvgResolutionHours(tickets),
                ClosedTicketsCount = closed.Count,
                TotalTicketsCount = tickets.Count,
                BucketGranularityLabel = granularityLabel,
                Buckets = buckets,
            };
        }

        private static UserRatingsReportDto BuildUserRatingsReport(
            List<DAL.Entities.Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo)
        {
            var rated = tickets.Where(t => t.Rating != null).ToList();

            var (granularity, granularityLabel) = FirstResponseReportHelper.ResolveGranularity(period, periodFrom, periodTo);
            var ranges = FirstResponseReportHelper.GenerateRanges(periodFrom, periodTo, granularity);

            var buckets = ranges.Select(r =>
            {
                var inBucket = rated.Where(t => t.CreatedDate >= r.Start && t.CreatedDate < r.End).ToList();
                return new RatingBucketDto
                {
                    Label = r.Label,
                    Count = inBucket.Count,
                    AvgRating = inBucket.Count > 0
                        ? Math.Round(inBucket.Average(t => (double)t.Rating!.RatingValue), 2)
                        : null,
                };
            }).ToList();

            if (rated.Count == 0)
                return new UserRatingsReportDto
                {
                    BucketGranularityLabel = granularityLabel,
                    Buckets = buckets,
                };

            var distribution = rated
                .GroupBy(t => t.Rating!.RatingValue)
                .Select(g => new RatingDistributionDto { Stars = g.Key, Count = g.Count() })
                .OrderBy(d => d.Stars)
                .ToList();

            return new UserRatingsReportDto
            {
                AverageRating = rated.Average(t => (double)t.Rating!.RatingValue),
                RatedTicketsCount = rated.Count,
                Distribution = distribution,
                BucketGranularityLabel = granularityLabel,
                Buckets = buckets,
            };
        }
    }
}
