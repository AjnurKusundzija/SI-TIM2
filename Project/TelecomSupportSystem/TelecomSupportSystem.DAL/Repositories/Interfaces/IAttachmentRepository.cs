using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        Task AddAsync(Attachment attachment);
        Task AddRangeAsync(IEnumerable<Attachment> attachments);
        Task<Attachment?> GetByIdAsync(int id);

        // PB-56 / US-81: dohvati prilog sa svim navigacijama (Ticket + Comment + Comment.Ticket + Ticket.Assignments + User)
        Task<Attachment?> GetByIdWithRelationsAsync(int id);
    }
}
