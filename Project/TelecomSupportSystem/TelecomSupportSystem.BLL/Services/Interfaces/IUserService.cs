using TelecomSupportSystem.BLL.DTOs.Users;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<AgentStatisticsDto> GetMyStatisticsAsync(int userId, string role);
        Task<IEnumerable<RecentTicketDto>> GetRecentAssignedTicketsAsync(int userId);
        Task<UserProfileDto> GetMyProfileAsync(int userId);
        Task UpdateEmailAsync(int userId, UpdateEmailDto dto);
        Task UpdatePasswordAsync(int userId, UpdatePasswordDto dto);
    }
}
