using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IFaqRepository
    {
        Task<IEnumerable<Faq>> GetActiveAsync();

        // PB-61: Admin pregled — sve FAQ stavke (uključujući neaktivne)
        Task<IEnumerable<Faq>> GetAllAsync();

        Task<Faq?> GetByIdAsync(int faqId);

        Task<Faq> CreateAsync(Faq faq);

        Task UpdateAsync(Faq faq);

        Task DeleteAsync(Faq faq);
    }
}
