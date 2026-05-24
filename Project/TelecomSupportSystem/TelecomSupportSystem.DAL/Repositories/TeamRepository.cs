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
    }
}
