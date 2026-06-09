using TelecomSupportSystem.BLL.DTOs.Sla;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.Services
{
    public class SlaService : ISlaService
    {
        // SLA deadlines in hours per priority (US-115)
        private static readonly Dictionary<Priority, double> SlaHours = new()
        {
            { Priority.CRITICAL, 2.0 },
            { Priority.HIGH,     8.0 },
            { Priority.MEDIUM,  24.0 },
            { Priority.LOW,     72.0 },
        };

        public SlaInfoDto GetSlaInfo(Ticket ticket)
        {
            if (!SlaHours.TryGetValue(ticket.Priority, out var totalHours))
                totalHours = 24.0;

            var deadline = ticket.CreatedDate.AddHours(totalHours);
            var now = DateTime.UtcNow;
            var remainingMinutes = (deadline - now).TotalMinutes;
            var totalMinutes = totalHours * 60.0;
            var percentRemaining = Math.Max(0, remainingMinutes / totalMinutes * 100.0);
            var isBreached = remainingMinutes <= 0;

            var status = percentRemaining switch
            {
                > 50 => "GREEN",
                > 20 => "YELLOW",
                _    => "RED",
            };

            return new SlaInfoDto
            {
                Deadline         = deadline,
                RemainingMinutes = remainingMinutes,
                PercentRemaining = percentRemaining,
                Status           = status,
                IsBreached       = isBreached,
            };
        }

        public int CountBreaches(IEnumerable<Ticket> tickets)
        {
            return tickets.Count(t =>
                t.Status != TicketStatus.CLOSED && GetSlaInfo(t).IsBreached);
        }
    }
}
