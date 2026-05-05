using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        // US-11
        Task<IEnumerable<Ticket>> GetByCreatorIdAsync(int creatorId);

        // US-29: Svi tiketi sa paginacijom (agent/administrator)
        Task<(IEnumerable<Ticket> Items, int TotalCount)> GetAllPagedAsync(int page, int pageSize);

        // US-30: Detalji tiketa sa kreatorom i komentarima
        Task<Ticket?> GetByIdWithDetailsAsync(int ticketId);

        Task<Ticket?> GetByIdAsync(int ticketId);
        Task<Ticket> CreateAsync(Ticket ticket);
    }
}
