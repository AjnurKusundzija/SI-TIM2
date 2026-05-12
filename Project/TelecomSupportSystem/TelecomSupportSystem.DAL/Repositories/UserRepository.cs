using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<IEnumerable<User>> GetAvailableAgentsByTeamIdAsync(int teamId)
            => await _context.Users
                .Where(u => u.TeamId == teamId
                         && u.Role == Role.AGENT
                         && u.AvailabilityStatus == AvailabilityStatus.AVAILABLE)
                .Include(u => u.TicketAssignments)
                    .ThenInclude(ta => ta.Ticket)
                .ToListAsync();

        // US-55, US-56: Svi dostupni agenti osim trenutnog vlasnika, s tiketima i ocjenama za izračun score-a
        public async Task<IEnumerable<User>> GetAvailableAgentsForForwardingAsync(int excludeUserId)
            => await _context.Users
                .Where(u => u.Role == Role.AGENT
                         && u.AvailabilityStatus == AvailabilityStatus.AVAILABLE
                         && u.AccountStatus == AccountStatus.ACTIVE
                         && u.UserId != excludeUserId)
                .Include(u => u.TicketAssignments)
                    .ThenInclude(ta => ta.Ticket)
                        .ThenInclude(t => t.Rating)
                .ToListAsync();
    }
}
