using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ITicketRepository _ticketRepository;

        public UserService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<AgentStatisticsDto> GetMyStatisticsAsync(int userId, string role)
        {
            var tickets = await _ticketRepository.GetAssignedTicketsForStatsAsync(userId);
            var ticketList = tickets.ToList();

            var openCount = ticketList.Count(t => t.Status == TicketStatus.OPEN);
            var closedCount = ticketList.Count(t => t.Status == TicketStatus.CLOSED);
            var pendingCount = ticketList.Count(t => t.Status == TicketStatus.CLOSURE_REQUESTED);

            // Prosječno vrijeme prvog odgovora: od kreiranja tiketa do prve poruke
            // bilo kojeg non-CLIENT korisnika (standardna helpdesk metrika)
            var firstResponseMinutes = ticketList
                .Select(t =>
                {
                    var firstStaffComment = t.Comments
                        .Where(c => c.Author.Role != Role.CLIENT)
                        .OrderBy(c => c.DateTime)
                        .FirstOrDefault();
                    if (firstStaffComment == null) return (double?)null;
                    return (firstStaffComment.DateTime - t.CreatedDate).TotalMinutes;
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            double? avgFirstResponse = firstResponseMinutes.Count > 0
                ? firstResponseMinutes.Average()
                : null;

            // Prosječno vrijeme rješavanja: od kreiranja do zatvaranja
            var resolutionHours = ticketList
                .Where(t => t.Status == TicketStatus.CLOSED && t.ClosedDate.HasValue)
                .Select(t => (t.ClosedDate!.Value - t.CreatedDate).TotalHours)
                .ToList();

            double? avgResolution = resolutionHours.Count > 0
                ? resolutionHours.Average()
                : null;

            // Prosječna ocjena — samo za agente
            double? avgRating = null;
            if (role == "AGENT")
            {
                var ratings = ticketList
                    .Where(t => t.Rating != null)
                    .Select(t => (double)t.Rating!.RatingValue)
                    .ToList();
                avgRating = ratings.Count > 0 ? ratings.Average() : null;
            }

            return new AgentStatisticsDto
            {
                OpenTicketsCount = openCount,
                ClosedTicketsCount = closedCount,
                PendingClosureCount = pendingCount,
                AvgFirstResponseMinutes = avgFirstResponse,
                AvgResolutionHours = avgResolution,
                AvgRating = avgRating
            };
        }

        public async Task<IEnumerable<RecentTicketDto>> GetRecentAssignedTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetRecentAssignedTicketsAsync(userId, 5);

            return tickets.Select(t => new RecentTicketDto
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                LastActivityDate = t.Comments.Any()
                    ? t.Comments.Max(c => c.DateTime)
                    : t.CreatedDate
            });
        }
    }
}
