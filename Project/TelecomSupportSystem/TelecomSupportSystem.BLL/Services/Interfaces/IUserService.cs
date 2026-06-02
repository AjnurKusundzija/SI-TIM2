using TelecomSupportSystem.BLL.DTOs.Users;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<AgentStatisticsDto> GetMyStatisticsAsync(int userId, string role);
        Task<IEnumerable<RecentTicketDto>> GetRecentAssignedTicketsAsync(int userId);
        Task<UserProfileDto> GetMyProfileAsync(int userId);
        Task<UserProfileDto> GetUserProfileAsync(int userId, int requestingUserId, string role);
        Task UpdateEmailAsync(int userId, UpdateEmailDto dto);
        Task UpdatePasswordAsync(int userId, UpdatePasswordDto dto);

        // PB-51: User Account Management
        Task CreateUserAsync(CreateUserDto dto, string currentRole, int? currentUserId = null, string? currentUserEmail = null);
        Task UpdateUserDetailsAsync(int targetUserId, UpdateUserDetailsDto dto, string currentRole, int? currentUserId = null);
        Task ChangeUserStatusAsync(int targetUserId, bool isActive, string currentRole, int currentUserId);
        Task<UserListDto> GetUsersPaginatedAsync(string currentRole, string? roleFilter, string? statusFilter, string? availabilityFilter, string? search, string? location, int page, int pageSize);
        Task SetAvailabilityAsync(int userId, string availability, string role, int actingUserId);
        Task<IEnumerable<TelecomSupportSystem.BLL.DTOs.Teams.TeamDto>> GetAgentTeamsAsync();
    }
}
