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
    }
}
