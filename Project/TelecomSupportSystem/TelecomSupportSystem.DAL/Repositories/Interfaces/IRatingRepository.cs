using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating?> GetByTicketIdAsync(int ticketId);
        Task<Rating> CreateAsync(Rating rating);
    }
}
