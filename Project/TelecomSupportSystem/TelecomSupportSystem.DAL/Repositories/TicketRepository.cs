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
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
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
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByAssigneeIdAsync(int userId)
        {
            var latestAssignedIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId &&
                             !_context.Set<TicketUser>().Any(tu2 =>
                                 tu2.TicketId == tu.TicketId &&
                                 tu2.AssignmentDate > tu.AssignmentDate))
                .Select(tu => tu.TicketId)
                .ToListAsync();

            return await _context.Tickets
                .Where(t => latestAssignedIds.Contains(t.TicketId))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // US-53: Otvoreni tiketi gdje je POSLJEDNJA dodjela na korisnika
        public async Task<IEnumerable<Ticket>> GetOpenAssignedTicketsAsync(int userId)
        {
            var latestAssignedIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId &&
                             !_context.Set<TicketUser>().Any(tu2 =>
                                 tu2.TicketId == tu.TicketId &&
                                 tu2.AssignmentDate > tu.AssignmentDate))
                .Select(tu => tu.TicketId)
                .ToListAsync();

            return await _context.Tickets
                .Where(t => latestAssignedIds.Contains(t.TicketId) && t.Status == TicketStatus.OPEN)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // US-54: Zatvoreni tiketi gdje je POSLJEDNJA dodjela bila na korisnika
        public async Task<IEnumerable<Ticket>> GetClosedAssignedTicketsAsync(int userId)
        {
            var latestAssignedIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId &&
                             !_context.Set<TicketUser>().Any(tu2 =>
                                 tu2.TicketId == tu.TicketId &&
                                 tu2.AssignmentDate > tu.AssignmentDate))
                .Select(tu => tu.TicketId)
                .ToListAsync();

            return await _context.Tickets
                .Where(t => latestAssignedIds.Contains(t.TicketId) && t.Status == TicketStatus.CLOSED)
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
            var latestAssignedIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId &&
                             !_context.Set<TicketUser>().Any(tu2 =>
                                 tu2.TicketId == tu.TicketId &&
                                 tu2.AssignmentDate > tu.AssignmentDate))
                .Select(tu => tu.TicketId)
                .ToListAsync();

            var tickets = await _context.Tickets
                .Where(t => latestAssignedIds.Contains(t.TicketId))
                .Include(t => t.Comments)
                .ToListAsync();

            return tickets
                .OrderByDescending(t => t.Comments.Any()
                    ? t.Comments.Max(c => c.DateTime)
                    : t.CreatedDate)
                .Take(count)
                .ToList();
        }

        // PB-42: Tiketi gdje je korisnik posljednji dodjeljeni (svi statusi), s komentarima i ocjenama
        public async Task<IEnumerable<Ticket>> GetAssignedTicketsForStatsAsync(int userId)
        {
            var latestAssignedIds = await _context.Set<TicketUser>()
                .Where(tu => tu.UserId == userId &&
                             !_context.Set<TicketUser>().Any(tu2 =>
                                 tu2.TicketId == tu.TicketId &&
                                 tu2.AssignmentDate > tu.AssignmentDate))
                .Select(tu => tu.TicketId)
                .ToListAsync();

            return await _context.Tickets
                .Where(t => latestAssignedIds.Contains(t.TicketId))
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .Include(t => t.Rating)
                .Include(t => t.Creator)
                .ToListAsync();
        }
    }
}
