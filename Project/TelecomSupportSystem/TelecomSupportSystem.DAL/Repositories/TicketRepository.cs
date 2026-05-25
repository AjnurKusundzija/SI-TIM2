using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // US-11: Vraća samo tikete čiji je CreatorId jednak proslijeđenom userId.
        // WHERE klauzula garantuje da korisnik ne može vidjeti tuđe tikete.
        public async Task<IEnumerable<Ticket>> GetByCreatorIdAsync(int creatorId)
        {
            return await _context.Tickets
                .Where(t => t.CreatorId == creatorId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int ticketId)
        {
            return await _context.Tickets.FindAsync(ticketId);
        }

        public async Task<Ticket?> GetByIdWithDetailsAsync(int ticketId)
        {
            return await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Assignments)
                    .ThenInclude(a => a.User)
                .Include(t => t.Attachments)
                    .ThenInclude(a => a.User)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Attachments)
                        .ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Assignments)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        private async Task<List<int>> GetActiveAssignedTicketIdsAsync(int userId)
        {
            var ticketIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId)
                .Select(tu => tu.TicketId)
                .Distinct()
                .ToListAsync();

            if (ticketIds.Count == 0)
                return new List<int>();

            var assignments = await _context.Set<TicketUser>()
                .Where(tu => ticketIds.Contains(tu.TicketId))
                .AsNoTracking()
                .ToListAsync();

            return assignments
                .GroupBy(tu => tu.TicketId)
                .Where(group =>
                {
                    var ordered = group
                        .OrderBy(tu => tu.AssignmentDate)
                        .ThenBy(tu => tu.AssignmentId)
                        .ToList();

                    var latest = ordered.LastOrDefault();
                    if (latest is null)
                        return false;

                    if (latest.UserId == userId)
                        return true;

                    if (latest.AssignmentType != AssignmentType.FORWARDED_TO_TECHNICIAN)
                        return false;

                    var previous = ordered.Count > 1 ? ordered[^2] : null;
                    return previous?.UserId == userId;
                })
                .Select(group => group.Key)
                .ToList();
        }

        public async Task<IEnumerable<Ticket>> GetByAssigneeIdAsync(int userId)
        {
            var activeAssignedIds = await GetActiveAssignedTicketIdsAsync(userId);

            return await _context.Tickets
                .Where(t => activeAssignedIds.Contains(t.TicketId))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // US-53: Otvoreni tiketi gdje je korisnik aktivno dodijeljen
        public async Task<IEnumerable<Ticket>> GetOpenAssignedTicketsAsync(int userId)
        {
            var activeAssignedIds = await GetActiveAssignedTicketIdsAsync(userId);

            return await _context.Tickets
                .Where(t => activeAssignedIds.Contains(t.TicketId) && t.Status == TicketStatus.OPEN)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // US-54: Zatvoreni tiketi gdje je korisnik aktivno dodijeljen
        public async Task<IEnumerable<Ticket>> GetClosedAssignedTicketsAsync(int userId)
        {
            var activeAssignedIds = await GetActiveAssignedTicketIdsAsync(userId);

            return await _context.Tickets
                .Where(t => activeAssignedIds.Contains(t.TicketId) && t.Status == TicketStatus.CLOSED)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task AddAssignmentAsync(TicketUser assignment)
        {
            _context.Set<TicketUser>().Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        // Dashboard: N najrecentnijih tiketa po zadnjoj aktivnosti (komentar ili kreiranje)
        public async Task<IEnumerable<Ticket>> GetRecentAssignedTicketsAsync(int userId, int count)
        {
            var activeAssignedIds = await GetActiveAssignedTicketIdsAsync(userId);

            var tickets = await _context.Tickets
                .Where(t => activeAssignedIds.Contains(t.TicketId))
                .Include(t => t.Comments)
                .ToListAsync();

            return tickets
                .OrderByDescending(t => t.Comments.Any()
                    ? t.Comments.Max(c => c.DateTime)
                    : t.CreatedDate)
                .Take(count)
                .ToList();
        }

        // PB-42: Tiketi gdje je korisnik aktivno dodijeljen (svi statusi), s komentarima i ocjenama
        public async Task<IEnumerable<Ticket>> GetAssignedTicketsForStatsAsync(int userId)
        {
            var activeAssignedIds = await GetActiveAssignedTicketIdsAsync(userId);

            return await _context.Tickets
                .Where(t => activeAssignedIds.Contains(t.TicketId))
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Rating)
                .Include(t => t.Creator)
                .ToListAsync();
        }
    }
}
