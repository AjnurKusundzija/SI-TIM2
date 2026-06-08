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
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task<User?> GetByPhoneAsync(string phone)
        {
            var normalized = phone.Trim();
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == normalized);
        }

        public async Task<User?> GetByIdAsync(int userId)
            => await _context.Users.Include(u => u.Team).FirstOrDefaultAsync(u => u.UserId == userId);

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

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

        public async Task<IEnumerable<User>> GetTechniciansByLocationAsync(Location location)
            => await _context.Users
                .Where(u => u.Role == Role.TECHNICIAN
                         && u.Location == location
                         && u.AccountStatus == AccountStatus.ACTIVE)
                .Include(u => u.TicketAssignments)
                    .ThenInclude(ta => ta.Ticket)
                .ToListAsync();

        public async Task CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPaginatedAsync(Role? role, AccountStatus? status, TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus? availability, string? search, Location? location, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            if (status.HasValue)
                query = query.Where(u => u.AccountStatus == status.Value);

            if (availability.HasValue)
                query = query.Where(u => u.AvailabilityStatus == availability.Value);

            if (location.HasValue)
                query = query.Where(u => u.Location == location.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLowerInvariant();
                query = query.Where(u => 
                    u.FirstName.ToLower().Contains(lowerSearch) ||
                    u.LastName.ToLower().Contains(lowerSearch) ||
                    u.Email.ToLower().Contains(lowerSearch) ||
                    u.Phone.ToLower().Contains(lowerSearch));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.Team) // Include Team for Agent Category
                .ToListAsync();

            return (users, totalCount);
        }
    }
}
