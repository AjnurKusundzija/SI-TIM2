using TelecomCustomerSupportSystem.Application.DTOs.Tickets;

namespace TelecomCustomerSupportSystem.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDto?> GetByIdAsync(int tiketId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TicketDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TicketDto?> CreateAsync(CreateTicketDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(int tiketId, string status, CancellationToken cancellationToken = default);
}