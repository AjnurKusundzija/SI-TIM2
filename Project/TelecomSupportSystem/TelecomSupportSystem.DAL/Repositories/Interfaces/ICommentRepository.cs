using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        // US-15: Dohvata komentare za tiket hronološki (najstariji prvi)
        Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId);
    }
}
