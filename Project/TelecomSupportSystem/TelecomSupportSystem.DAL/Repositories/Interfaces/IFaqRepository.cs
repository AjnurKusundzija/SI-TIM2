using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IFaqRepository
    {
        Task<IEnumerable<Faq>> GetActiveAsync();
    }
}
