using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        // US-11
        Task<IEnumerable<Ticket>> GetByCreatorIdAsync(int creatorId);

        // Placeholderi za buduće US-ove (kreiranje, admin pregled, itd.)
        Task<Ticket?> GetByIdAsync(int ticketId);
        Task<Ticket> CreateAsync(Ticket ticket);
    }
}
