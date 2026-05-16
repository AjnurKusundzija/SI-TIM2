using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Tickets
{
    // US-25: Unit testovi logike automatske dodjele tiketa.
    // Pokrivaju sve acceptance kriterije US-25:
    //   AC1 — automatska dodjela kada postoje pravila + dostupan agent
    //   AC2 — nedostupan agent se ne smije izabrati
    //   AC4 — kada nema agenta ili tima, vraća se odgovarajuća poruka
    //   AC5 — bira se agent s najmanjim opterećenjem (broj tiketa, pa srednji prioritet)
    //   AC6 — poruka kada nema definisanih pravila (tim za kategoriju ne postoji)
    public class AutoAssignServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepoMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly TicketService _service;

        public AutoAssignServiceTests()
        {
            _service = new TicketService(_ticketRepoMock.Object, _teamRepoMock.Object, _userRepoMock.Object, _notificationServiceMock.Object);

            _ticketRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => { t.TicketId = 42; return t; });
        }

        private static CreateTicketDto MakeDto(ProblemCategory category = ProblemCategory.INTERNET, Priority priority = Priority.MEDIUM) => new()
        {
            Subject = "Tiket",
            Description = "Opis problema",
            Priority = priority,
            Type = category
        };

        private static User MakeAgent(int id, int teamId, AvailabilityStatus status = AvailabilityStatus.AVAILABLE, IEnumerable<TicketUser>? assignments = null) => new()
        {
            UserId = id,
            FirstName = $"Agent{id}",
            LastName = "Test",
            Email = $"a{id}@test.ba",
            Username = $"a{id}",
            PasswordHash = "h",
            Role = Role.AGENT,
            AccountStatus = AccountStatus.ACTIVE,
            AvailabilityStatus = status,
            TeamId = teamId,
            TicketAssignments = assignments?.ToList() ?? new List<TicketUser>()
        };

        private static TicketUser MakeAssignment(int ticketId, Priority priority) => new()
        {
            TicketId = ticketId,
            Ticket = new Ticket { TicketId = ticketId, Priority = priority }
        };

        // ─── AC1, AC5 ─────────────────────────────────────────────────────────────

        // US-25 / AC1: kada postoji tim za kategoriju i dostupan agent, sistem auto-dodjeljuje tiket
        [Fact]
        public async Task CreateTicketAsync_AutoAssignsTicket_WhenTeamAndAvailableAgentExist()
        {
            var team = new Team { TeamId = 1, TeamName = "Internet Tim", SpecializedCategory = ProblemCategory.INTERNET };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.INTERNET)).ReturnsAsync(team);

            _userRepoMock
                .Setup(r => r.GetAvailableAgentsByTeamIdAsync(1))
                .ReturnsAsync(new[] { MakeAgent(10, 1) });

            var result = await _service.CreateTicketAsync(MakeDto(), userId: 5);

            result.AssignedAgentName.Should().Be("Agent10 Test");
            result.AssignmentMessage.Should().BeNull();
            result.TeamId.Should().Be(1);
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.Is<TicketUser>(a =>
                a.UserId == 10 &&
                a.TeamId == 1 &&
                a.AssignmentType == AssignmentType.AUTOMATIC)), Times.Once);
        }

        // US-25 / AC5: bira agenta s najmanjim brojem dodijeljenih tiketa
        [Fact]
        public async Task CreateTicketAsync_PicksAgentWithFewestAssignments()
        {
            var team = new Team { TeamId = 2, TeamName = "TV Tim", SpecializedCategory = ProblemCategory.TV };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.TV)).ReturnsAsync(team);

            var busy = MakeAgent(20, 2, assignments: new[]
            {
                MakeAssignment(1, Priority.LOW),
                MakeAssignment(2, Priority.LOW),
                MakeAssignment(3, Priority.LOW),
            });
            var light = MakeAgent(21, 2, assignments: new[] { MakeAssignment(4, Priority.LOW) });

            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(2)).ReturnsAsync(new[] { busy, light });

            var result = await _service.CreateTicketAsync(MakeDto(ProblemCategory.TV), userId: 1);

            result.AssignedAgentName.Should().Be("Agent21 Test");
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.Is<TicketUser>(a => a.UserId == 21)), Times.Once);
        }

        // US-25 / AC5: kod jednakog broja tiketa bira agenta s manjim prosječnim prioritetom (manje opterećen)
        [Fact]
        public async Task CreateTicketAsync_BreaksTieByLowerMeanPriority()
        {
            var team = new Team { TeamId = 3, TeamName = "Mobilni Tim", SpecializedCategory = ProblemCategory.MOBILE_NETWORK };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.MOBILE_NETWORK)).ReturnsAsync(team);

            // Oba imaju 2 tiketa — heavyMean ima HIGH/HIGH, lightMean ima LOW/LOW; bira se lightMean
            var heavyMean = MakeAgent(30, 3, assignments: new[]
            {
                MakeAssignment(1, Priority.HIGH),
                MakeAssignment(2, Priority.HIGH),
            });
            var lightMean = MakeAgent(31, 3, assignments: new[]
            {
                MakeAssignment(3, Priority.LOW),
                MakeAssignment(4, Priority.LOW),
            });

            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(3)).ReturnsAsync(new[] { heavyMean, lightMean });

            var result = await _service.CreateTicketAsync(MakeDto(ProblemCategory.MOBILE_NETWORK), userId: 1);

            result.AssignedAgentName.Should().Be("Agent31 Test");
        }

        // US-25 / AC1: dodjela koristi tip AUTOMATIC i sadrži objašnjenje u Note polju
        [Fact]
        public async Task CreateTicketAsync_RecordsAssignmentAsAutomaticWithNote()
        {
            var team = new Team { TeamId = 4, TeamName = "Naplata Tim", SpecializedCategory = ProblemCategory.BILLING };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.BILLING)).ReturnsAsync(team);
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(4)).ReturnsAsync(new[] { MakeAgent(40, 4) });

            await _service.CreateTicketAsync(MakeDto(ProblemCategory.BILLING), userId: 1);

            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.Is<TicketUser>(a =>
                a.AssignmentType == AssignmentType.AUTOMATIC &&
                a.Note == "Automatska dodjela prema kategoriji tiketa")), Times.Once);
        }

        // ─── AC2 ──────────────────────────────────────────────────────────────────

        // US-25 / AC2: kada repository vrati 0 dostupnih agenata (npr. svi BUSY/UNAVAILABLE), ne dolazi do dodjele
        [Fact]
        public async Task CreateTicketAsync_DoesNotAssign_WhenNoAgentsAreAvailable()
        {
            var team = new Team { TeamId = 5, TeamName = "Tehnička", SpecializedCategory = ProblemCategory.TECHNICAL_SUPPORT };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.TECHNICAL_SUPPORT)).ReturnsAsync(team);
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(5)).ReturnsAsync(Array.Empty<User>());

            var result = await _service.CreateTicketAsync(MakeDto(ProblemCategory.TECHNICAL_SUPPORT), userId: 1);

            result.AssignedAgentName.Should().BeNull();
            result.AssignmentMessage.Should().Be("Nema dostupnih agenata. Tiket je označen kao Nedodijeljen.");
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Never);
        }

        // ─── AC6 ──────────────────────────────────────────────────────────────────

        // US-25 / AC6: kada ne postoji tim za kategoriju (nema definisanih pravila), vraća se poruka
        [Fact]
        public async Task CreateTicketAsync_ReturnsNoRulesMessage_WhenNoTeamMatchesCategory()
        {
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(It.IsAny<ProblemCategory>())).ReturnsAsync((Team?)null);

            var result = await _service.CreateTicketAsync(MakeDto(), userId: 1);

            result.AssignedAgentName.Should().BeNull();
            result.TeamId.Should().BeNull();
            result.AssignmentMessage.Should().Be("Nema definisanih pravila dodjele za odabranu kategoriju.");
            _userRepoMock.Verify(r => r.GetAvailableAgentsByTeamIdAsync(It.IsAny<int>()), Times.Never);
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Never);
        }

        // ─── Sigurnost / poslovna pravila ─────────────────────────────────────────

        // US-25: novi tiket uvijek persistira u bazi prije pokušaja dodjele (Create dolazi prije Assign)
        [Fact]
        public async Task CreateTicketAsync_PersistsTicketBeforeAttemptingAssignment()
        {
            var sequence = new MockSequence();
            _ticketRepoMock.InSequence(sequence).Setup(r => r.CreateAsync(It.IsAny<Ticket>())).ReturnsAsync((Ticket t) => { t.TicketId = 99; return t; });
            _ticketRepoMock.InSequence(sequence).Setup(r => r.AddAssignmentAsync(It.IsAny<TicketUser>())).Returns(Task.CompletedTask);

            var team = new Team { TeamId = 6, TeamName = "Internet Tim", SpecializedCategory = ProblemCategory.INTERNET };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.INTERNET)).ReturnsAsync(team);
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(6)).ReturnsAsync(new[] { MakeAgent(60, 6) });

            await _service.CreateTicketAsync(MakeDto(), userId: 1);

            _ticketRepoMock.Verify(r => r.CreateAsync(It.IsAny<Ticket>()), Times.Once);
            _ticketRepoMock.Verify(r => r.AddAssignmentAsync(It.IsAny<TicketUser>()), Times.Once);
        }

        // US-25: kreirani tiket dobija TeamId iz pronađenog tima (mapiranje kategorija→tim)
        [Theory]
        [InlineData(ProblemCategory.INTERNET)]
        [InlineData(ProblemCategory.TV)]
        [InlineData(ProblemCategory.MOBILE_NETWORK)]
        [InlineData(ProblemCategory.BILLING)]
        [InlineData(ProblemCategory.TECHNICAL_SUPPORT)]
        public async Task CreateTicketAsync_AssignsCorrectTeam_BasedOnCategory(ProblemCategory category)
        {
            var team = new Team { TeamId = 7, TeamName = $"Tim {category}", SpecializedCategory = category };
            _teamRepoMock.Setup(r => r.GetBySpecializedCategoryAsync(category)).ReturnsAsync(team);
            _userRepoMock.Setup(r => r.GetAvailableAgentsByTeamIdAsync(7)).ReturnsAsync(new[] { MakeAgent(70, 7) });

            var result = await _service.CreateTicketAsync(MakeDto(category), userId: 1);

            result.TeamId.Should().Be(7);
            _ticketRepoMock.Verify(r => r.CreateAsync(It.Is<Ticket>(t =>
                t.ProblemCategory == category &&
                t.TeamId == 7)), Times.Once);
        }
    }
}
