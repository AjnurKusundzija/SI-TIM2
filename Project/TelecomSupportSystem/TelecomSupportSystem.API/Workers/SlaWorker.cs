using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.API.Workers
{
    public class SlaWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SlaWorker> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);

        public SlaWorker(IServiceScopeFactory scopeFactory, ILogger<SlaWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckSlaAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška u SlaWorker iteraciji.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }

        private async Task CheckSlaAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var ticketRepo = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
            var slaService = scope.ServiceProvider.GetRequiredService<ISlaService>();

            var tickets = await ticketRepo.GetOpenTicketsAsync();

            foreach (var ticket in tickets)
            {
                if (ct.IsCancellationRequested) break;

                var sla = slaService.GetSlaInfo(ticket);

                var assignedAgentId = ticket.Assignments
                    .OrderBy(a => a.AssignmentDate)
                    .LastOrDefault(a => a.User?.Role == Role.AGENT)?.UserId;

                if (assignedAgentId is null) continue;

                if (sla.IsBreached)
                {
                    // Log breach once per ticket
                    var alreadyLogged = await auditLogService.ExistsAsync(
                        AuditActionType.SLA_BREACH, ticket.TicketId.ToString());

                    if (!alreadyLogged)
                    {
                        await auditLogService.LogAsync(
                            AuditActionType.SLA_BREACH,
                            "Ticket",
                            ticket.TicketId.ToString(),
                            $"SLA prekoračenje tiketa #{ticket.TicketId} \"{ticket.Title}\" (prioritet: {ticket.Priority})");
                    }

                    // Send breach notification once per ticket
                    if (!await notificationRepo.ExistsAsync(ticket.TicketId, NotificationType.SLA_BREACH))
                    {
                        await notificationService.SendNotificationAsync(
                            assignedAgentId.Value,
                            "⚠ SLA prekoračenje",
                            $"Tiket #{ticket.TicketId} \"{ticket.Title}\" je prekoračio SLA rok.",
                            NotificationType.SLA_BREACH,
                            ticket.TicketId);
                    }
                }
                else if (sla.PercentRemaining <= 20)
                {
                    // Send warning notification once per ticket
                    if (!await notificationRepo.ExistsAsync(ticket.TicketId, NotificationType.SLA_WARNING))
                    {
                        var remaining = sla.RemainingMinutes < 60
                            ? $"{Math.Round(sla.RemainingMinutes)} min"
                            : $"{Math.Round(sla.RemainingMinutes / 60, 1)}h";

                        await notificationService.SendNotificationAsync(
                            assignedAgentId.Value,
                            "⏰ SLA upozorenje",
                            $"Tiket #{ticket.TicketId} \"{ticket.Title}\" ističe za {remaining}.",
                            NotificationType.SLA_WARNING,
                            ticket.TicketId);
                    }
                }
            }
        }
    }
}
