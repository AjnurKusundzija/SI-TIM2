using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int userId);
        Task UpdateAsync(User user);

        // US-25: Dohvata dostupne agente u timu za automatsku dodjelu tiketa
        Task<IEnumerable<User>> GetAvailableAgentsByTeamIdAsync(int teamId);

        // US-55, US-56: Dohvata sve dostupne agente za prosljeđivanje (s historijom tiketa za scoring)
        Task<IEnumerable<User>> GetAvailableAgentsForForwardingAsync(int excludeUserId);

        // US-TechnicianForwarding: Dohvata tehničare na određenoj lokaciji sa njihovim zaduženjima
        Task<IEnumerable<User>> GetTechniciansByLocationAsync(Location location);
    }
}
