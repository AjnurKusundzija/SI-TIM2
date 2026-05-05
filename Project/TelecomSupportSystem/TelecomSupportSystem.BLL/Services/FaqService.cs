using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _faqRepository;

        public FaqService(IFaqRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }

        public async Task<IEnumerable<GetFaqDto>> GetFaqsAsync()
        {
            var faqs = await _faqRepository.GetActiveAsync();

            return faqs.Select(faq => new GetFaqDto
            {
                FaqId = faq.FaqId,
                Question = faq.Question,
                Answer = faq.Answer,
                Category = faq.Category,
                SortOrder = faq.SortOrder
            });
        }
    }
}
