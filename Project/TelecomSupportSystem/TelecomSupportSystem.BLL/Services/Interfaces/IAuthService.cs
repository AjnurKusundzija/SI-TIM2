using TelecomSupportSystem.BLL.DTOs.Auth;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto, string? ipAddress = null);
        Task<RefreshResponseDto?> RefreshAsync(string refreshToken);
        Task<bool> RevokeAsync(string refreshToken);
    }
}
