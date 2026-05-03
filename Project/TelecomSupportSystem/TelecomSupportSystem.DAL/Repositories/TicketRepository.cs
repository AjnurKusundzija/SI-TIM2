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

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }
    }
}
