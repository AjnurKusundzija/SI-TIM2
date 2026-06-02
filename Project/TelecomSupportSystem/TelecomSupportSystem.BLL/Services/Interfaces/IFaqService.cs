using TelecomSupportSystem.BLL.DTOs.Faq;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IFaqService
    {
        Task<IEnumerable<GetFaqDto>> GetFaqsAsync();

        // PB-61: Admin lista — uključuje neaktivne stavke
        Task<IEnumerable<GetFaqDto>> GetAllFaqsAsync();

        Task<GetFaqDto> CreateFaqAsync(CreateFaqDto dto);

        Task<GetFaqDto> UpdateFaqAsync(int faqId, UpdateFaqDto dto);

        Task DeleteFaqAsync(int faqId);
    }
}
