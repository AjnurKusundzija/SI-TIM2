using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        // US-25: Dohvata dostupne agente u timu za automatsku dodjelu tiketa
        Task<IEnumerable<User>> GetAvailableAgentsByTeamIdAsync(int teamId);
    }
}
