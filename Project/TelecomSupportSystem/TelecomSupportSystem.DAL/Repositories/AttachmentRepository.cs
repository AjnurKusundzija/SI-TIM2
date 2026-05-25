using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Attachment attachment)
        {
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Attachment> attachments)
        {
            await _context.Attachments.AddRangeAsync(attachments);
            await _context.SaveChangesAsync();
        }

        public async Task<Attachment?> GetByIdAsync(int id)
        {
            return await _context.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == id);
        }

        public async Task<Attachment?> GetByIdWithRelationsAsync(int id)
        {
            return await _context.Attachments
                .Include(a => a.User)
                .Include(a => a.Ticket)
                    .ThenInclude(t => t!.Assignments)
                .Include(a => a.Comment)
                    .ThenInclude(c => c!.Ticket)
                        .ThenInclude(t => t.Assignments)
                .FirstOrDefaultAsync(a => a.AttachmentId == id);
        }
    }
}
