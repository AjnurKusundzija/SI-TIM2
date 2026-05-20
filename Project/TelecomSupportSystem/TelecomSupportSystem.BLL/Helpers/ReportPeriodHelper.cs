namespace TelecomSupportSystem.BLL.Helpers
{
    public static class ReportPeriodHelper
    {
        public static (DateTime From, DateTime To, string Label) Resolve(string period, DateTime? from, DateTime? to)
        {
            var now = DateTime.UtcNow;

            return period?.ToLowerInvariant() switch
            {
                "week" => (now.AddDays(-7), now, "Sedmica"),
                "month" => (now.AddDays(-30), now, "Mjesec"),
                "year" => (now.AddDays(-365), now, "Godina"),
                "custom" when from.HasValue && to.HasValue =>
                    ValidateCustom(from.Value, to.Value),
                _ => throw new ArgumentException("Neispravan vremenski period. Dozvoljeno: week, month, year, custom."),
            };
        }

        private static (DateTime From, DateTime To, string Label) ValidateCustom(DateTime from, DateTime to)
        {
            if (to < from)
                throw new ArgumentException("Datum kraja mora biti nakon datuma početka.");

            return (from.ToUniversalTime(), to.ToUniversalTime(), "Prilagođeno");
        }

        public static bool IsLargePeriod(DateTime from, DateTime to) =>
            (to - from).TotalDays > 90;
    }
}
