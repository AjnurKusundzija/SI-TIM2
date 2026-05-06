using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
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

        public async Task<IEnumerable<Ticket>> GetByCreatorIdAsync(int creatorId)
        {
            return await _context.Tickets
                .Where(t => t.CreatorId == creatorId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // US-29
        public async Task<(IEnumerable<Ticket> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize)
        {
            var query = _context.Tickets
                .Include(t => t.Creator)
                .OrderByDescending(t => t.CreatedDate);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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
                .FirstOrDefaultAsync(t => t.TicketId == ticketId);
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }
    }
}
