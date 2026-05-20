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

        public async Task<IReadOnlyList<AgentResolveRow>> GetAgentResolvedCountsAsync(DateTime from, DateTime to)
        {
            var closed = await _context.Tickets
                .Where(t =>
                    t.Status == TicketStatus.CLOSED
                    && t.ClosedDate.HasValue
                    && t.ClosedDate >= from
                    && t.ClosedDate <= to
                    && t.ClosedById.HasValue)
                .GroupBy(t => t.ClosedById!.Value)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToListAsync();

            if (closed.Count == 0)
                return Array.Empty<AgentResolveRow>();

            var userIds = closed.Select(c => c.UserId).ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .Where(u => u.Role == Role.AGENT || u.Role == Role.TECHNICIAN)
                .AsNoTracking()
                .ToListAsync();

            return closed
                .Join(users, c => c.UserId, u => u.UserId, (c, u) => new AgentResolveRow
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    ResolvedCount = c.Count,
                })
                .OrderByDescending(r => r.ResolvedCount)
                .ToList();
        }
    }
}
