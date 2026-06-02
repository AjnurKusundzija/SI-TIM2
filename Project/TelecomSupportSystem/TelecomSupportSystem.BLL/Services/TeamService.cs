using Microsoft.Extensions.Logging;
using TelecomSupportSystem.BLL.DTOs.Teams;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            ITicketRepository ticketRepository,
            IAuditLogService auditLogService,
            ILogger<TeamService> logger)
        {
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _auditLogService = auditLogService;
            _logger = logger;
        }

        public async Task<IEnumerable<TeamOverviewDto>> GetAllTeamsOverviewAsync()
        {
            var teams = await _teamRepository.GetAllWithMembersAsync();
            var result = new List<TeamOverviewDto>();

            foreach (var team in teams)
            {
                // Active members are already filtered in the repository (AccountStatus.ACTIVE)
                var activeMembers = team.Members.ToList();

                // Build member DTOs — get open ticket count per member
                var memberDtos = new List<TeamMemberDto>();
                foreach (var member in activeMembers)
                {
                    var openTickets = await _ticketRepository.GetOpenAssignedTicketsAsync(member.UserId);
                    memberDtos.Add(new TeamMemberDto
                    {
                        UserId = member.UserId,
                        FirstName = member.FirstName,
                        LastName = member.LastName,
                        ExpertiseCategory = team.SpecializedCategory?.ToString() ?? string.Empty,
                        Availability = member.AvailabilityStatus?.ToString(),
                        OpenTicketCount = openTickets.Count()
                    });
                }

                result.Add(new TeamOverviewDto
                {
                    TeamId = team.TeamId,
                    TeamName = team.TeamName,
                    SpecializedCategory = team.SpecializedCategory?.ToString() ?? string.Empty,
                    ActiveAgentCount = activeMembers.Count,
                    OpenTicketCount = team.Tickets.Count + memberDtos.Sum(m => m.OpenTicketCount),
                    Members = memberDtos
                });
            }

            return result;
        }

        public async Task ReassignAgentAsync(int agentId, int newTeamId, int adminId, string? ipAddress = null)
        {
            // 1. Load agent
            var agent = await _userRepository.GetByIdAsync(agentId);
            if (agent == null)
                throw new KeyNotFoundException("Agent nije pronađen.");

            // 2. Verify agent is active
            if (agent.AccountStatus != AccountStatus.ACTIVE)
                throw new InvalidOperationException("Nije moguće premjestiti neaktivnog agenta.");

            // 3. Check same team
            if (agent.TeamId == newTeamId)
                throw new InvalidOperationException("Agent je već u odabranom timu.");

            // 4. Backend validation: agent must have no open tickets
            var openTickets = await _ticketRepository.GetOpenAssignedTicketsAsync(agentId);
            if (openTickets.Any())
                throw new InvalidOperationException(
                    $"Agent {agent.FirstName} {agent.LastName} ima {openTickets.Count()} otvorenih tiketa. Premještanje nije moguće dok postoje otvoreni tiketi.");

            // 5. Load new team
            var newTeam = await _teamRepository.GetByIdAsync(newTeamId);
            if (newTeam == null)
                throw new KeyNotFoundException("Odabrani tim nije pronađen.");

            // 6. Load old team info for audit log
            string? oldTeamName = null;
            if (agent.TeamId.HasValue)
            {
                var oldTeam = await _teamRepository.GetByIdAsync(agent.TeamId.Value);
                oldTeamName = oldTeam?.TeamName;
            }

            var oldTeamId = agent.TeamId;

            // 7. Load admin for audit description
            var admin = await _userRepository.GetByIdAsync(adminId);
            var adminName = admin != null ? $"{admin.FirstName} {admin.LastName}" : $"Admin #{adminId}";

            // 8. Perform reassignment
            agent.TeamId = newTeamId;
            await _userRepository.UpdateAsync(agent);

            // 9. Write audit log
            await _auditLogService.LogAsync(
                AuditActionType.AGENT_REASSIGNED,
                "User",
                agentId.ToString(),
                $"Administrator {adminName} premjestio agenta {agent.FirstName} {agent.LastName} iz tima \"{oldTeamName ?? "N/A"}\" u tim \"{newTeam.TeamName}\"",
                userId: adminId,
                oldValue: new { teamId = oldTeamId, teamName = oldTeamName },
                newValue: new { teamId = newTeamId, teamName = newTeam.TeamName },
                ipAddress: ipAddress);

            _logger.LogInformation(
                "Agent {AgentId} reassigned from team {OldTeamId} to team {NewTeamId} by admin {AdminId}",
                agentId, oldTeamId, newTeamId, adminId);
        }
    }
}
