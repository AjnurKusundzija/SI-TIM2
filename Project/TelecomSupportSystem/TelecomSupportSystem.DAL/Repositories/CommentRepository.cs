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

        // US-15: Komentari za tiket, hronološki (najstariji prvi), sa autorom
        public async Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.Comments
                .Include(c => c.Author)
                .Include(c => c.Attachments)
                .Where(c => c.TicketId == ticketId && !c.IsInternal)
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
