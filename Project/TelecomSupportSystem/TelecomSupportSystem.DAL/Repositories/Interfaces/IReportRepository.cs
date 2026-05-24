using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IReadOnlyList<Ticket>> GetTicketsCreatedInPeriodAsync(DateTime from, DateTime to);
        Task<UserRoleCounts> GetActiveUserCountsByRoleAsync();
        Task<int> GetOpenTicketsCountAsync();
        Task<int> GetClosureRequestedCountAsync();
        Task<int> GetUnassignedOpenTicketsCountAsync();
        Task<int> GetStaleTicketsCountAsync(DateTime olderThanUtc);
        Task<IReadOnlyList<AgentResolveRow>> GetAgentResolvedCountsAsync(DateTime from, DateTime to);
        Task<IReadOnlyList<AgentTicketResolutionRow>> GetAgentResolvedDetailsAsync(DateTime from, DateTime to);
        Task<int> GetClosedInPeriodCountAsync(DateTime from, DateTime to);
    }

    public class UserRoleCounts
    {
        public int Clients { get; set; }
        public int Agents { get; set; }
        public int Technicians { get; set; }
        public int Administrators { get; set; }
    }

    public class AgentResolveRow
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Role Role { get; set; }
        public int ResolvedCount { get; set; }
    }

    public class AgentTicketResolutionRow
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Role Role { get; set; }
        public DateTime ClosedDate { get; set; }
        public int TicketId { get; set; }
    }
}
