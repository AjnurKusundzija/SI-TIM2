using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.BLL.Helpers
{
    public enum ReportBucketGranularity
    {
        Day,
        Week,
        Month,
    }

    public static class FirstResponseReportHelper
    {
        public static (ReportBucketGranularity Granularity, string Label) ResolveGranularity(
            string period,
            DateTime from,
            DateTime to)
        {
            return period.ToLowerInvariant() switch
            {
                "week" => (ReportBucketGranularity.Day, "Po danu"),
                "month" => (ReportBucketGranularity.Week, "Po sedmici"),
                "year" => (ReportBucketGranularity.Month, "Po mjesecu"),
                _ => (to - from).TotalDays switch
                {
                    <= 14 => (ReportBucketGranularity.Day, "Po danu"),
                    <= 90 => (ReportBucketGranularity.Week, "Po sedmici"),
                    _ => (ReportBucketGranularity.Month, "Po mjesecu"),
                },
            };
        }

        public static FirstResponseReportDto Build(
            List<Ticket> tickets,
            string period,
            DateTime periodFrom,
            DateTime periodTo)
        {
            var (granularity, granularityLabel) = ResolveGranularity(period, periodFrom, periodTo);
            var buckets = BuildBuckets(tickets, periodFrom, periodTo, granularity);

            var withResponse = tickets
                .Select(TicketMetricsHelper.GetFirstResponseMinutes)
                .Where(x => x.HasValue)
                .ToList();

            return new FirstResponseReportDto
            {
                AvgFirstResponseMinutes = TicketMetricsHelper.CalculateAvgFirstResponseMinutes(tickets),
                TotalTicketsCount = tickets.Count,
                TicketsWithResponseCount = withResponse.Count,
                BucketGranularity = granularity.ToString().ToLowerInvariant(),
                BucketGranularityLabel = granularityLabel,
                Buckets = buckets,
            };
        }

        private static IReadOnlyList<FirstResponseBucketDto> BuildBuckets(
            List<Ticket> tickets,
            DateTime periodFrom,
            DateTime periodTo,
            ReportBucketGranularity granularity)
        {
            var ranges = GenerateRanges(periodFrom, periodTo, granularity);
            var result = new List<FirstResponseBucketDto>();

            foreach (var (start, end, label) in ranges)
            {
                var inBucket = tickets
                    .Where(t => t.CreatedDate >= start && t.CreatedDate < end)
                    .ToList();

                var responseMinutes = inBucket
                    .Select(TicketMetricsHelper.GetFirstResponseMinutes)
                    .Where(x => x.HasValue)
                    .Select(x => x!.Value)
                    .ToList();

                result.Add(new FirstResponseBucketDto
                {
                    Label = label,
                    PeriodStart = start,
                    PeriodEnd = end,
                    TicketCount = inBucket.Count,
                    TicketsWithResponseCount = responseMinutes.Count,
                    AvgFirstResponseMinutes = responseMinutes.Count > 0
                        ? Math.Round(responseMinutes.Average(), 1)
                        : null,
                });
            }

            return result;
        }

        private static List<(DateTime Start, DateTime End, string Label)> GenerateRanges(
            DateTime from,
            DateTime to,
            ReportBucketGranularity granularity)
        {
            var ranges = new List<(DateTime, DateTime, string)>();
            var cursor = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);

            while (cursor < to)
            {
                DateTime end;
                string label;

                switch (granularity)
                {
                    case ReportBucketGranularity.Day:
                        end = cursor.AddDays(1);
                        label = cursor.ToString("dd.MM.yyyy");
                        break;
                    case ReportBucketGranularity.Week:
                        end = cursor.AddDays(7);
                        if (end > to) end = to;
                        label = $"{cursor:dd.MM} – {end.AddDays(-1):dd.MM}";
                        break;
                    default:
                        end = cursor.AddMonths(1);
                        if (end > to) end = to;
                        label = cursor.ToString("MM/yyyy");
                        break;
                }

                if (end > to) end = to;
                if (cursor < end)
                    ranges.Add((cursor, end, label));

                cursor = end;
            }

            return ranges;
        }
    }
}
