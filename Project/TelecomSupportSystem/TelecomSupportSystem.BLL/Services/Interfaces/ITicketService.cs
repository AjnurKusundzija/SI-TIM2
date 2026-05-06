using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ITicketService
    {
        // US-11
        Task<IEnumerable<MyTicketDto>> GetMyTicketsAsync(int userId);

        // PB-22
        Task<GetTicketDto> CreateTicketAsync(CreateTicketDto createTicketDto, int userId);

        // US-14, US-30: Detaljan prikaz tiketa — pristup ovisi o roli
        Task<TicketDetailDto> GetTicketByIdAsync(int ticketId, int userId, string role);
    }
}
