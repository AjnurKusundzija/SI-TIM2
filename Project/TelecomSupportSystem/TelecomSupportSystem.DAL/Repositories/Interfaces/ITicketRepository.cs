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
    }
}
