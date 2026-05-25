using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public AttachmentRepository()
        {
            // Konstruktor
        }

        public async Task AddAsync(Attachment attachment)
        {
            await Task.CompletedTask;
        }

        public async Task AddRangeAsync(IEnumerable<Attachment> attachments)
        {
            await Task.CompletedTask;
        }

        public async Task<Attachment?> GetByIdAsync(int id)
        {
            // Vraća null ili simulirani Task dok se ne spoji kompletan DbContext query
            return await Task.FromResult<Attachment?>(null);
        }
    }
}