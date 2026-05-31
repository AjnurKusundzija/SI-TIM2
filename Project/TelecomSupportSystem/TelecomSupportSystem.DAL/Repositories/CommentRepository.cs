using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // US-15 / US-103: Komentari za tiket, hronološki (najstariji prvi), sa autorom.
        // includeInternal=false → vraća samo regularne poruke (npr. za klijenta).
        // includeInternal=true  → vraća i interne komentare (osoblje: AGENT/TECHNICIAN/ADMINISTRATOR).
        public async Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId, bool includeInternal = false)
        {
            var query = _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                    .ThenInclude(a => a.User)
                .Where(c => c.TicketId == ticketId);

            if (!includeInternal)
                query = query.Where(c => !c.IsInternal);

            return await query
                .OrderBy(c => c.DateTime)
                .ToListAsync();
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }
    }
}
