using TelecomSupportSystem.BLL.DTOs.Teams;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ITeamService
    {
        /// <summary>
        /// Returns all teams with their active members and current open ticket counts.
        /// </summary>
        Task<IEnumerable<TeamOverviewDto>> GetAllTeamsOverviewAsync();

        /// <summary>
        /// Reassigns an agent to a new team.
        /// Throws KeyNotFoundException if agent or team not found.
        /// Throws InvalidOperationException if agent is inactive, already in target team, or has open tickets.
        /// </summary>
        Task ReassignAgentAsync(int agentId, int newTeamId, int adminId, string? ipAddress = null);
    }
}
