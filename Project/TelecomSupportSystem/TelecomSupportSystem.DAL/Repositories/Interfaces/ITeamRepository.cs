using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team?> GetBySpecializedCategoryAsync(ProblemCategory category);
        Task<IEnumerable<Team>> GetAgentTeamsAsync();

        /// <summary>
        /// Returns all teams with their active members (AccountStatus == ACTIVE) included.
        /// </summary>
        Task<IEnumerable<Team>> GetAllWithMembersAsync();

        /// <summary>
        /// Returns a single team by ID with its active members included.
        /// </summary>
        Task<Team?> GetByIdAsync(int teamId);
    }
}
