using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.DAL.Entities.Enums;

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

        // PB-32: Lista svih tiketa filtrirana prema roli korisnika; assignedOnly filtrira samo dodijeljene (AGENT)
        Task<IEnumerable<MyTicketDto>> GetAllTicketsAsync(int userId, string role, bool assignedOnly = false);

        // US-53: Otvoreni tiketi dodijeljeni agentu
        Task<IEnumerable<MyTicketDto>> GetOpenAssignedTicketsAsync(int userId);

        // US-54: Zatvoreni tiketi koji su bili dodijeljeni agentu
        Task<IEnumerable<MyTicketDto>> GetClosedAssignedTicketsAsync(int userId);

        // US-56: Dohvata listu dostupnih agenata sa izračunatim score-ovima za ručni odabir
        Task<IEnumerable<AgentScoreDto>> GetAgentScoresAsync(int ticketId, int currentAgentId);

        // US-55: Automatski proslijedi tiket agentu s najvišim score-om
        Task<AgentScoreDto> AutoForwardTicketAsync(int ticketId, int currentAgentId);

        // US-56: Proslijedi tiket odabranom agentu
        Task<AgentScoreDto> ForwardTicketToAgentAsync(int ticketId, int targetAgentId, int currentAgentId);

        // US-TechnicianForwarding: Proslijedi tiket tehničaru na određenoj lokaciji
        Task<AgentScoreDto> ForwardTicketToTechnicianAsync(int ticketId, string location, int currentAgentId);

        // Internal Priority Management
        Task UpdateInternalPriorityAsync(int ticketId, InternalPriority priority, int userId, string role);
    }
}
