using TelecomCustomerSupportSystem.Application.DTOs.Users;

namespace TelecomCustomerSupportSystem.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int korisnikId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UserDto userDto, CancellationToken cancellationToken = default);
}