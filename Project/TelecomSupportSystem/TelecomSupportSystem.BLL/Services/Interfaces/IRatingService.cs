using TelecomSupportSystem.BLL.DTOs.Ratings;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IRatingService
    {
        Task<RatingDto?> GetRatingForTicketAsync(int ticketId, int requestingUserId, string role);
        Task<RatingDto> CreateRatingAsync(int ticketId, int userId, string role, CreateRatingDto dto);
    }
}
