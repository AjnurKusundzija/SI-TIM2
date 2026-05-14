using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    // US-25: Unit testovi repozitorija koji podržavaju auto-dodjelu.
    // Pokriva: GetBySpecializedCategoryAsync, GetAvailableAgentsByTeamIdAsync, AddAssignmentAsync.
    public class AutoAssignRepositoryTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static User MakeAgent(int id, int teamId, AvailabilityStatus status = AvailabilityStatus.AVAILABLE, Role role = Role.AGENT) => new()
        {
            UserId = id,
            FirstName = $"Agent{id}",
            LastName = "Test",
            Email = $"a{id}@test.ba",
            Username = $"a{id}",
            PasswordHash = "h",
            Role = role,
            AccountStatus = AccountStatus.ACTIVE,
            AvailabilityStatus = status,
            TeamId = teamId,
        };

        // US-25 / AC5: GetBySpecializedCategoryAsync vraća tim za zadanu kategoriju
        [Fact]
        public async Task GetBySpecializedCategoryAsync_ReturnsMatchingTeam()
        {
            using var context = CreateDbContext();
            context.Teams.AddRange(
                new Team { TeamId = 1, TeamName = "Internet Tim", TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.INTERNET },
                new Team { TeamId = 2, TeamName = "TV Tim",       TeamType = TeamType.AGENTS, TeamStatus = TeamStatus.ACTIVE, SpecializedCategory = ProblemCategory.TV }
            );
            await context.SaveChangesAsync();

            var repo = new TeamRepository(context);
            var team = await repo.GetBySpecializedCategoryAsync(ProblemCategory.TV);

            team.Should().NotBeNull();
            team!.TeamName.Should().Be("TV Tim");
        }

        // US-25 / AC6: GetBySpecializedCategoryAsync vraća null kada nema tima — to je signal "nema pravila"
        [Fact]
        public async Task GetBySpecializedCategoryAsync_ReturnsNull_WhenNoTeamHasCategory()
        {
            using var context = CreateDbContext();
            context.Teams.Add(new Team
            {
                TeamId = 1,
                TeamName = "Internet Tim",
                TeamType = TeamType.AGENTS,
                TeamStatus = TeamStatus.ACTIVE,
                SpecializedCategory = ProblemCategory.INTERNET
            });
            await context.SaveChangesAsync();

            var repo = new TeamRepository(context);
            var team = await repo.GetBySpecializedCategoryAsync(ProblemCategory.BILLING);

            team.Should().BeNull();
        }

        // US-25 / AC2: vraća samo agente s AvailabilityStatus.AVAILABLE iz traženog tima
        [Fact]
        public async Task GetAvailableAgentsByTeamIdAsync_ReturnsOnlyAvailableAgents_InTeam()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                MakeAgent(10, teamId: 1, status: AvailabilityStatus.AVAILABLE),
                MakeAgent(11, teamId: 1, status: AvailabilityStatus.BUSY),
                MakeAgent(12, teamId: 1, status: AvailabilityStatus.UNAVAILABLE),
                MakeAgent(13, teamId: 1, status: AvailabilityStatus.AVAILABLE),
                // Drugi tim — ne smije biti vraćen
                MakeAgent(20, teamId: 2, status: AvailabilityStatus.AVAILABLE)
            );
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);
            var agents = (await repo.GetAvailableAgentsByTeamIdAsync(1)).ToList();

            agents.Should().HaveCount(2);
            agents.Select(a => a.UserId).Should().BeEquivalentTo(new[] { 10, 13 });
        }

        // US-25 / AC2: vraća samo korisnike s Role.AGENT (tehničar/admin/klijent se ne smiju vratiti)
        [Fact]
        public async Task GetAvailableAgentsByTeamIdAsync_DoesNotReturnNonAgents()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                MakeAgent(10, teamId: 1, status: AvailabilityStatus.AVAILABLE, role: Role.AGENT),
                MakeAgent(11, teamId: 1, status: AvailabilityStatus.AVAILABLE, role: Role.TECHNICIAN),
                MakeAgent(12, teamId: 1, status: AvailabilityStatus.AVAILABLE, role: Role.ADMINISTRATOR),
                MakeAgent(13, teamId: 1, status: AvailabilityStatus.AVAILABLE, role: Role.CLIENT)
            );
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);
            var agents = (await repo.GetAvailableAgentsByTeamIdAsync(1)).ToList();

            agents.Should().ContainSingle();
            agents[0].UserId.Should().Be(10);
        }

        // US-25 / AC5: učitava TicketAssignments + Ticket kako bi servis mogao sortirati po opterećenju
        [Fact]
        public async Task GetAvailableAgentsByTeamIdAsync_IncludesAssignmentsAndTickets_ForLoadSorting()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeAgent(10, teamId: 1));

            var t1 = new Ticket { Title = "T1", Description = "d", CreatorId = 999, Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow };
            var t2 = new Ticket { Title = "T2", Description = "d", CreatorId = 999, Status = TicketStatus.OPEN, Priority = Priority.LOW,  ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow };
            context.Tickets.AddRange(t1, t2);
            context.Users.Add(new User
            {
                UserId = 999, FirstName = "C", LastName = "C", Email = "c@c.ba", Username = "c", PasswordHash = "h",
                Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE
            });
            await context.SaveChangesAsync();

            context.Set<TicketUser>().AddRange(
                new TicketUser { TicketId = t1.TicketId, UserId = 10, TeamId = 1, AssignmentDate = DateTime.UtcNow, AssignmentType = AssignmentType.MANUAL, Note = "" },
                new TicketUser { TicketId = t2.TicketId, UserId = 10, TeamId = 1, AssignmentDate = DateTime.UtcNow, AssignmentType = AssignmentType.MANUAL, Note = "" }
            );
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);
            var agents = (await repo.GetAvailableAgentsByTeamIdAsync(1)).ToList();

            var agent = agents.Single();
            agent.TicketAssignments.Should().HaveCount(2);
            agent.TicketAssignments.Select(a => a.Ticket).Should().NotContainNulls();
            agent.TicketAssignments.Select(a => (int)a.Ticket.Priority).Should()
                .BeEquivalentTo(new[] { (int)Priority.HIGH, (int)Priority.LOW });
        }

        // US-25 / AC1: AddAssignmentAsync persistira novi TicketUser zapis
        [Fact]
        public async Task AddAssignmentAsync_PersistsAssignment()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeAgent(10, teamId: 1));
            context.Users.Add(new User
            {
                UserId = 1, FirstName = "C", LastName = "C", Email = "c@c.ba", Username = "c", PasswordHash = "h",
                Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE
            });
            var ticket = new Ticket { Title = "T", Description = "d", CreatorId = 1, Status = TicketStatus.OPEN, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var repo = new TicketRepository(context);
            await repo.AddAssignmentAsync(new TicketUser
            {
                TicketId = ticket.TicketId,
                UserId = 10,
                TeamId = 1,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.AUTOMATIC,
                Note = "test"
            });

            context.Set<TicketUser>().Should().ContainSingle(a =>
                a.TicketId == ticket.TicketId &&
                a.UserId == 10 &&
                a.AssignmentType == AssignmentType.AUTOMATIC);
        }

        // US-25 / AC3: nakon AddAssignmentAsync, GetByAssigneeIdAsync vraća tiket za tog agenta
        [Fact]
        public async Task AddAssignmentAsync_MakesTicketVisibleToAssignedAgent()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeAgent(10, teamId: 1));
            context.Users.Add(new User
            {
                UserId = 1, FirstName = "C", LastName = "C", Email = "c@c.ba", Username = "c", PasswordHash = "h",
                Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE
            });
            var ticket = new Ticket { Title = "Visible to agent", Description = "d", CreatorId = 1, Status = TicketStatus.OPEN, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = DateTime.UtcNow };
            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            var repo = new TicketRepository(context);
            await repo.AddAssignmentAsync(new TicketUser
            {
                TicketId = ticket.TicketId,
                UserId = 10,
                TeamId = 1,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.AUTOMATIC,
                Note = ""
            });

            var agentTickets = (await repo.GetByAssigneeIdAsync(10)).ToList();
            agentTickets.Should().ContainSingle()
                .Which.Title.Should().Be("Visible to agent");
        }
    }
}
