using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    /// <summary>
    /// US-41 through US-48 – Reports and dashboard statistics unit tests.
    /// Tests UserService.GetMyStatisticsAsync which aggregates agent workload data.
    /// </summary>
    public class AgentStatisticsServiceTests
    {
        private readonly Mock<ITicketRepository>  _ticketRepoMock     = new();
        private readonly Mock<IUserRepository>    _userRepoMock       = new();
        private readonly Mock<IPackageService>    _packageServiceMock = new();
        private readonly UserService              _service;

        private const int AgentId = 1;

        public AgentStatisticsServiceTests()
        {
            _service = new UserService(
                _ticketRepoMock.Object,
                _userRepoMock.Object,
                _packageServiceMock.Object);
        }

        private static User MakeUser(int id, Role role) => new()
        {
            UserId        = id,
            FirstName     = "Test",
            LastName      = "User",
            Role          = role,
            AccountStatus = AccountStatus.ACTIVE,
        };

        private static Ticket MakeTicketWithStatus(
            int id,
            TicketStatus status,
            DateTime? closedDate = null,
            DateTime? createdDate = null) => new()
        {
            TicketId        = id,
            Title           = $"Ticket {id}",
            CreatorId       = 99,
            Status          = status,
            Priority        = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate     = createdDate ?? DateTime.UtcNow.AddHours(-2),
            ClosedDate      = closedDate,
            Comments        = new List<Comment>(),
            Rating          = null,
            Assignments     = new List<TicketUser>(),
        };

        // ─── Ticket count grouped by status ────────────────────────────────

        /// <summary>
        /// US-41: Statistics correctly group ticket counts by status.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldReturnCorrectStatusCounts_WhenMixedStatuses()
        {
            var tickets = new[]
            {
                MakeTicketWithStatus(1, TicketStatus.OPEN),
                MakeTicketWithStatus(2, TicketStatus.OPEN),
                MakeTicketWithStatus(3, TicketStatus.CLOSED, closedDate: DateTime.UtcNow.AddHours(-1)),
                MakeTicketWithStatus(4, TicketStatus.CLOSURE_REQUESTED),
            };
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.OpenTicketsCount.Should().Be(2);
            result.ClosedTicketsCount.Should().Be(1);
            result.PendingClosureCount.Should().Be(1);
        }

        /// <summary>
        /// US-41: When no tickets exist, all counts are zero.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldReturnAllZeros_WhenNoTickets()
        {
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.OpenTicketsCount.Should().Be(0);
            result.ClosedTicketsCount.Should().Be(0);
            result.PendingClosureCount.Should().Be(0);
        }

        // ─── Average resolution time (only for CLOSED tickets) ─────────────

        /// <summary>
        /// US-44: Average resolution time calculated only for CLOSED tickets.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldCalculateAvgResolution_ForClosedTicketsOnly()
        {
            var created  = DateTime.UtcNow.AddHours(-4);
            var closedAt = DateTime.UtcNow.AddHours(-2);  // 2h resolution time

            var tickets = new[]
            {
                MakeTicketWithStatus(1, TicketStatus.CLOSED, closedDate: closedAt, createdDate: created),
                MakeTicketWithStatus(2, TicketStatus.OPEN),  // open ticket — excluded from avg
            };
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.AvgResolutionHours.Should().BeApproximately(2.0, 0.1);
        }

        /// <summary>
        /// US-44: Average resolution time is null when no CLOSED tickets exist.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldReturnNullAvgResolution_WhenNoClosedTickets()
        {
            var tickets = new[]
            {
                MakeTicketWithStatus(1, TicketStatus.OPEN),
                MakeTicketWithStatus(2, TicketStatus.CLOSURE_REQUESTED),
            };
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.AvgResolutionHours.Should().BeNull();
        }

        // ─── Agent workload: open ticket count per agent ────────────────────

        /// <summary>
        /// US-48: Agent workload report returns correct open ticket count for the agent.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldCountOpenTickets_AsAgentWorkload()
        {
            var tickets = new[]
            {
                MakeTicketWithStatus(1, TicketStatus.OPEN),
                MakeTicketWithStatus(2, TicketStatus.OPEN),
                MakeTicketWithStatus(3, TicketStatus.OPEN),
                MakeTicketWithStatus(4, TicketStatus.CLOSED, closedDate: DateTime.UtcNow),
            };
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.OpenTicketsCount.Should().Be(3);
        }

        // ─── Average rating included for AGENT role only ────────────────────

        /// <summary>
        /// US-45: Average rating is calculated for AGENT role when rated tickets exist.
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldCalculateAvgRating_WhenAgentHasRatedTickets()
        {
            var tickets = new[]
            {
                new Ticket
                {
                    TicketId        = 1,
                    Status          = TicketStatus.CLOSED,
                    CreatorId       = 99,
                    Priority        = Priority.LOW,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate     = DateTime.UtcNow.AddHours(-5),
                    ClosedDate      = DateTime.UtcNow.AddHours(-1),
                    Comments        = new List<Comment>(),
                    Assignments     = new List<TicketUser>(),
                    Rating          = new TelecomSupportSystem.DAL.Entities.Rating
                    {
                        RatingId    = 1,
                        TicketId    = 1,
                        UserId      = 99,
                        RatingValue = 4,
                        RatingDate  = DateTime.UtcNow,
                    }
                },
                new Ticket
                {
                    TicketId        = 2,
                    Status          = TicketStatus.CLOSED,
                    CreatorId       = 99,
                    Priority        = Priority.LOW,
                    ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate     = DateTime.UtcNow.AddHours(-3),
                    ClosedDate      = DateTime.UtcNow.AddHours(-1),
                    Comments        = new List<Comment>(),
                    Assignments     = new List<TicketUser>(),
                    Rating          = new TelecomSupportSystem.DAL.Entities.Rating
                    {
                        RatingId    = 2,
                        TicketId    = 2,
                        UserId      = 99,
                        RatingValue = 2,
                        RatingDate  = DateTime.UtcNow,
                    }
                }
            };
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetMyStatisticsAsync(AgentId, "AGENT");

            result.AvgRating.Should().BeApproximately(3.0, 0.01); // (4 + 2) / 2
        }

        /// <summary>
        /// US-45: Average rating is null for TECHNICIAN role (only agents get ratings).
        /// </summary>
        [Fact]
        public async Task GetMyStatisticsAsync_ShouldReturnNullAvgRating_ForTechnicianRole()
        {
            _ticketRepoMock.Setup(r => r.GetAssignedTicketsForStatsAsync(AgentId))
                .ReturnsAsync(Enumerable.Empty<Ticket>());

            var result = await _service.GetMyStatisticsAsync(AgentId, "TECHNICIAN");

            result.AvgRating.Should().BeNull();
        }
    }
}
