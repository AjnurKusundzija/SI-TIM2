using TelecomCustomerSupportSystem.Application.DTOs.Auth;

namespace TelecomCustomerSupportSystem.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(object request, CancellationToken cancellationToken = default);
}