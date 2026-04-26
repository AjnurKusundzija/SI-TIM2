using TelecomSupportSystem.BLL.DTOs.Auth;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<RefreshResponseDto?> RefreshAsync(string refreshToken);
        Task<bool> RevokeAsync(string refreshToken);
    }
}
