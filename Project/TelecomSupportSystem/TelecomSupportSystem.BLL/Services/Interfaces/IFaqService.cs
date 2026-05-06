using TelecomSupportSystem.BLL.DTOs.Faq;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IFaqService
    {
        Task<IEnumerable<GetFaqDto>> GetFaqsAsync();
    }
}
