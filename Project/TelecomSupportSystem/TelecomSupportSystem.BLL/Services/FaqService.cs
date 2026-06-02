using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class FaqService : IFaqService
    {
        // PB-61: konzistentne poruke validacije
        public const string QuestionRequiredMessage = "Pitanje ne smije biti prazno.";
        public const string AnswerRequiredMessage = "Odgovor ne smije biti prazan.";

        private readonly IFaqRepository _faqRepository;

        public FaqService(IFaqRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }

        public async Task<IEnumerable<GetFaqDto>> GetFaqsAsync()
        {
            var faqs = await _faqRepository.GetActiveAsync();

            return faqs.Select(MapToDto);
        }

        public async Task<IEnumerable<GetFaqDto>> GetAllFaqsAsync()
        {
            var faqs = await _faqRepository.GetAllAsync();

            return faqs.Select(MapToDto);
        }

        public async Task<GetFaqDto> CreateFaqAsync(CreateFaqDto dto)
        {
            ValidateContent(dto.Question, dto.Answer);

            var faq = new Faq
            {
                Question = dto.Question.Trim(),
                Answer = dto.Answer.Trim(),
                Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim(),
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _faqRepository.CreateAsync(faq);
            return MapToDto(created);
        }

        public async Task<GetFaqDto> UpdateFaqAsync(int faqId, UpdateFaqDto dto)
        {
            ValidateContent(dto.Question, dto.Answer);

            var faq = await _faqRepository.GetByIdAsync(faqId)
                ?? throw new KeyNotFoundException($"FAQ stavka {faqId} nije pronađena.");

            faq.Question = dto.Question.Trim();
            faq.Answer = dto.Answer.Trim();
            faq.Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim();
            faq.SortOrder = dto.SortOrder;

            await _faqRepository.UpdateAsync(faq);
            return MapToDto(faq);
        }

        public async Task DeleteFaqAsync(int faqId)
        {
            var faq = await _faqRepository.GetByIdAsync(faqId)
                ?? throw new KeyNotFoundException($"FAQ stavka {faqId} nije pronađena.");

            await _faqRepository.DeleteAsync(faq);
        }

        private static void ValidateContent(string question, string answer)
        {
            if (string.IsNullOrWhiteSpace(question))
                throw new ArgumentException(QuestionRequiredMessage);

            if (string.IsNullOrWhiteSpace(answer))
                throw new ArgumentException(AnswerRequiredMessage);
        }

        private static GetFaqDto MapToDto(Faq faq) => new()
        {
            FaqId = faq.FaqId,
            Question = faq.Question,
            Answer = faq.Answer,
            Category = faq.Category,
            SortOrder = faq.SortOrder
        };
    }
}
