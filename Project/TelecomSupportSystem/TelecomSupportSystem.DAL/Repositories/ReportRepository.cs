using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Ticket>> GetTicketsCreatedInPeriodAsync(DateTime from, DateTime to)
        {
            return await _context.Tickets
                .Where(t => t.CreatedDate >= from && t.CreatedDate <= to)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Rating)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserRoleCounts> GetActiveUserCountsByRoleAsync()
        {
            var grouped = await _context.Users
                .Where(u => u.AccountStatus == AccountStatus.ACTIVE)
                .GroupBy(u => u.Role)
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToListAsync();

            return new UserRoleCounts
            {
                Clients = grouped.FirstOrDefault(x => x.Role == Role.CLIENT)?.Count ?? 0,
                Agents = grouped.FirstOrDefault(x => x.Role == Role.AGENT)?.Count ?? 0,
                Technicians = grouped.FirstOrDefault(x => x.Role == Role.TECHNICIAN)?.Count ?? 0,
                Administrators = grouped.FirstOrDefault(x => x.Role == Role.ADMINISTRATOR)?.Count ?? 0,
            };
        }

        public Task<int> GetOpenTicketsCountAsync() =>
            _context.Tickets.CountAsync(t => t.Status == TicketStatus.OPEN);

        public Task<int> GetClosureRequestedCountAsync() =>
            _context.Tickets.CountAsync(t => t.Status == TicketStatus.CLOSURE_REQUESTED);

        public async Task<int> GetUnassignedOpenTicketsCountAsync()
        {
            var assignedIds = await _context.TicketUsers
                .Select(tu => tu.TicketId)
                .Distinct()
                .ToListAsync();

            return await _context.Tickets.CountAsync(t =>
                t.Status == TicketStatus.OPEN && !assignedIds.Contains(t.TicketId));
        }

        public Task<int> GetStaleTicketsCountAsync(DateTime olderThanUtc) =>
            _context.Tickets.CountAsync(t =>
                (t.Status == TicketStatus.OPEN || t.Status == TicketStatus.CLOSURE_REQUESTED)
                && t.CreatedDate <= olderThanUtc);

        public Task<int> GetClosedInPeriodCountAsync(DateTime from, DateTime to) =>
            _context.Tickets.CountAsync(t =>
                t.Status == TicketStatus.CLOSED
                && t.ClosedDate.HasValue
                && t.ClosedDate >= from
                && t.ClosedDate <= to);

        public async Task<IReadOnlyList<AgentResolveRow>> GetAgentResolvedCountsAsync(DateTime from, DateTime to)
        {
            var closedTicketIds = await _context.Tickets
                .Where(t =>
                    t.Status == TicketStatus.CLOSED
                    && t.ClosedDate.HasValue
                    && t.ClosedDate >= from
                    && t.ClosedDate <= to)
                .Select(t => t.TicketId)
                .ToListAsync();

            if (closedTicketIds.Count == 0)
                return Array.Empty<AgentResolveRow>();

            var assignments = await _context.TicketUsers
                .Where(tu => closedTicketIds.Contains(tu.TicketId))
                .Join(
                    _context.Users.Where(u => u.Role == Role.AGENT || u.Role == Role.TECHNICIAN),
                    tu => tu.UserId,
                    u => u.UserId,
                    (tu, u) => new { tu.TicketId, tu.UserId, tu.AssignmentDate, u.FirstName, u.LastName, u.Role })
                .ToListAsync();

            // Za agente: po tiketu računa se samo posljednji assignirani agent
            // (kad agent proslijedi drugom agentu, stari gubi bod)
            var agentCredits = assignments
                .Where(x => x.Role == Role.AGENT)
                .GroupBy(x => x.TicketId)
                .Select(g => g.OrderByDescending(x => x.AssignmentDate).First());

            // Za tehničare: svi assignirani tehničari dobivaju bod
            var techCredits = assignments.Where(x => x.Role == Role.TECHNICIAN);

            return agentCredits.Concat(techCredits)
                .GroupBy(x => new { x.UserId, x.FirstName, x.LastName, x.Role })
                .Select(g => new AgentResolveRow
                {
                    UserId = g.Key.UserId,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    Role = g.Key.Role,
                    ResolvedCount = g.Select(x => x.TicketId).Distinct().Count(),
                })
                .OrderByDescending(r => r.ResolvedCount)
                .ToList();
        }
    }
}
