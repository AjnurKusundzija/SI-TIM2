namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class AgentStatisticsDto
    {
        public int OpenTicketsCount { get; set; }
        public int ClosedTicketsCount { get; set; }
        public int PendingClosureCount { get; set; }
        public double? AvgFirstResponseMinutes { get; set; }
        public double? AvgResolutionHours { get; set; }
        public double? AvgRating { get; set; }
    }
}
