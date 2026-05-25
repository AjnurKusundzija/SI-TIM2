using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        Task AddAsync(Attachment attachment);
        Task AddRangeAsync(IEnumerable<Attachment> attachments);
        Task<Attachment?> GetByIdAsync(int id); // Dodana metoda koju AttachmentsController traži
    }
}