using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    // PB-56: no-op fallback koji koriste TicketService/CommentService stari overload-i
    // (testovi koji ne testiraju attachment funkcionalnost i ne moraju mockovati IAttachmentRepository).
    public class NullAttachmentRepository : IAttachmentRepository
    {
        public Task AddAsync(Attachment attachment) => Task.CompletedTask;
        public Task AddRangeAsync(IEnumerable<Attachment> attachments) => Task.CompletedTask;
        public Task<Attachment?> GetByIdAsync(int id) => Task.FromResult<Attachment?>(null);
        public Task<Attachment?> GetByIdWithRelationsAsync(int id) => Task.FromResult<Attachment?>(null);
    }
}
