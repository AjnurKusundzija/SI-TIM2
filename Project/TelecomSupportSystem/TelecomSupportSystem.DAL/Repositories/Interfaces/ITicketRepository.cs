using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        // US-11
        Task<IEnumerable<Ticket>> GetByCreatorIdAsync(int creatorId);

        Task<Ticket?> GetByIdAsync(int ticketId);

        // Dohvata tiket s Creator i Assignments.User za prikaz detalja i provjeru pristupa
        Task<Ticket?> GetByIdWithDetailsAsync(int ticketId);

        Task<Ticket> CreateAsync(Ticket ticket);

        // PB-32: Svi tiketi (za ADMINISTRATOR/AGENT)
        Task<IEnumerable<Ticket>> GetAllAsync();

        // PB-32: Tiketi dodijeljeni korisniku (za TECHNICIAN)
        Task<IEnumerable<Ticket>> GetByAssigneeIdAsync(int userId);

        // US-53: Otvoreni tiketi dodijeljeni korisniku (za AGENT)
        Task<IEnumerable<Ticket>> GetOpenAssignedTicketsAsync(int userId);

        // US-54: Zatvoreni tiketi koji su bili dodijeljeni korisniku (za AGENT)
        Task<IEnumerable<Ticket>> GetClosedAssignedTicketsAsync(int userId);

        // US-25: Kreira zapis o automatskoj dodjeli tiketa agentu
        Task AddAssignmentAsync(TicketUser assignment);
    }
}
