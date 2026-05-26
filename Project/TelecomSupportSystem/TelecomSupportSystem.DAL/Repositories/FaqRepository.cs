using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly ApplicationDbContext _context;

        public FaqRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Faq>> GetActiveAsync()
        {
            return await _context.Faqs
                .AsNoTracking()
                .Where(faq => faq.IsActive)
                .OrderBy(faq => faq.SortOrder)
                .ThenBy(faq => faq.FaqId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Faq>> GetAllAsync()
        {
            return await _context.Faqs
                .AsNoTracking()
                .OrderBy(faq => faq.SortOrder)
                .ThenBy(faq => faq.FaqId)
                .ToListAsync();
        }

        public async Task<Faq?> GetByIdAsync(int faqId)
        {
            return await _context.Faqs.FirstOrDefaultAsync(faq => faq.FaqId == faqId);
        }

        public async Task<Faq> CreateAsync(Faq faq)
        {
            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task UpdateAsync(Faq faq)
        {
            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Faq faq)
        {
            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
        }
    }
}
