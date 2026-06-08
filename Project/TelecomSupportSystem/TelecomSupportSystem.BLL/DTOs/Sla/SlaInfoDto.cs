namespace TelecomSupportSystem.BLL.DTOs.Sla
{
    public class SlaInfoDto
    {
        public DateTime Deadline { get; init; }
        public double RemainingMinutes { get; init; }
        public double PercentRemaining { get; init; }
        /// <summary>GREEN (&gt;50%), YELLOW (20–50%), RED (&lt;20% or breached)</summary>
        public string Status { get; init; } = "GREEN";
        public bool IsBreached { get; init; }
    }
}
