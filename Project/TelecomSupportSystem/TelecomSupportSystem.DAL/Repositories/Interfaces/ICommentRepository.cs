using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        // US-15 / US-103: Dohvata komentare za tiket hronološki (najstariji prvi).
        // includeInternal=true uključuje i interne komentare (vraća se osoblju).
        Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId, bool includeInternal = false);

        Task<Comment> CreateAsync(Comment comment);
    }
}
