using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.DAL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly ApplicationDbContext _context;

        public TeamRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Team?> GetBySpecializedCategoryAsync(ProblemCategory category)
            => await _context.Teams.FirstOrDefaultAsync(t => t.SpecializedCategory == category);

        public async Task<IEnumerable<Team>> GetAgentTeamsAsync()
        {
            return await _context.Teams
                .Where(t => t.TeamType == TeamType.AGENTS)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetAllWithMembersAsync()
        {
            return await _context.Teams
                .Include(t => t.Members.Where(m => m.AccountStatus == AccountStatus.ACTIVE))
                .Include(t => t.Tickets.Where(ticket => ticket.Status == TicketStatus.OPEN))
                .OrderBy(t => t.TeamName)
                .ToListAsync();
        }

        public async Task<Team?> GetByIdAsync(int teamId)
        {
            return await _context.Teams
                .Include(t => t.Members.Where(m => m.AccountStatus == AccountStatus.ACTIVE))
                .FirstOrDefaultAsync(t => t.TeamId == teamId);
        }
    }
}
