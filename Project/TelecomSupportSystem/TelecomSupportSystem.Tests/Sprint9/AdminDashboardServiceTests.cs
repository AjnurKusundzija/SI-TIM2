using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.Helpers;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-45 — Admin dashboard + globalni filter + generisanje izvještaja
    // Pokriva US-71, US-72, US-82, US-83, US-85, US-86.
    public class AdminDashboardServiceTests
    {
        private readonly Mock<IReportRepository> _repo = new();
        private readonly ReportService _service;

        public AdminDashboardServiceTests()
        {
            _service = new ReportService(_repo.Object);
        }

        private static Ticket MakeTicket(int id, TicketStatus status, DateTime created, ProblemCategory category = ProblemCategory.INTERNET, int? rating = null)
        {
            var t = new Ticket
            {
                TicketId = id,
                Title = $"T{id}",
                CreatorId = 1,
                Description = "D",
                Status = status,
                Priority = Priority.MEDIUM,
                ProblemCategory = category,
                CreatedDate = created,
                ClosedDate = status == TicketStatus.CLOSED ? created.AddHours(2) : null,
                Comments = new List<Comment>(),
                Rating = rating.HasValue ? new DAL.Entities.Rating { RatingValue = rating.Value, UserId = 1, TicketId = id } : null,
            };
            return t;
        }

        private void SetupRepoEmptyBaseline()
        {
            _repo.Setup(r => r.GetActiveUserCountsByRoleAsync()).ReturnsAsync(new UserRoleCounts());
            _repo.Setup(r => r.GetAgentResolvedCountsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(Array.Empty<AgentResolveRow>());
            _repo.Setup(r => r.GetOpenTicketsCountAsync()).ReturnsAsync(0);
            _repo.Setup(r => r.GetClosureRequestedCountAsync()).ReturnsAsync(0);
            _repo.Setup(r => r.GetUnassignedOpenTicketsCountAsync()).ReturnsAsync(0);
            _repo.Setup(r => r.GetStaleTicketsCountAsync(It.IsAny<DateTime>())).ReturnsAsync(0);
            _repo.Setup(r => r.GetClosedInPeriodCountAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(0);
        }

        // ── US-71: Dashboard vraća sve must-have sekcije ───────────────────────

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldReturnAllMustHaveSections()
        {
            SetupRepoEmptyBaseline();
            var now = DateTime.UtcNow;
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[]
                {
                    MakeTicket(1, TicketStatus.OPEN, now.AddDays(-1)),
                    MakeTicket(2, TicketStatus.CLOSED, now.AddDays(-2), rating: 4),
                    MakeTicket(3, TicketStatus.CLOSURE_REQUESTED, now.AddDays(-3), category: ProblemCategory.BILLING),
                });

            var dto = await _service.GetAdminDashboardAsync("month", null, null);

            dto.Period.Should().NotBeNull();
            dto.TotalTicketsInPeriod.Should().Be(3);
            dto.StatusBreakdown.Should().NotBeEmpty();
            dto.TopProblemTypes.Should().NotBeEmpty();
            dto.ActiveUsersByRole.Should().NotBeNull();
            dto.AvgRating.Should().Be(4);
        }

        // ── US-71: Status agregati uključuju OPEN, CLOSED, CLOSURE_REQUESTED ───

        [Fact]
        public async Task GetAdminDashboardAsync_StatusBreakdown_ShouldIncludeAllValidStatuses_AndExcludeNonexistentCancelled()
        {
            SetupRepoEmptyBaseline();
            var now = DateTime.UtcNow;
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[]
                {
                    MakeTicket(1, TicketStatus.OPEN, now.AddDays(-1)),
                    MakeTicket(2, TicketStatus.CLOSED, now.AddDays(-2)),
                    MakeTicket(3, TicketStatus.CLOSURE_REQUESTED, now.AddDays(-3)),
                });

            var dto = await _service.GetAdminDashboardAsync("month", null, null);

            var statuses = dto.StatusBreakdown.Select(s => s.Status).ToList();
            statuses.Should().Contain(new[] { "OPEN", "CLOSED", "CLOSURE_REQUESTED" });
            statuses.Should().NotContain("CANCELLED");
        }

        // ── US-71: Kada nema podataka — broj 0 + null KPI vrijednosti ──────────

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldReturnZeroCounts_AndNullKpi_WhenNoTickets()
        {
            SetupRepoEmptyBaseline();
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Array.Empty<Ticket>());

            var dto = await _service.GetAdminDashboardAsync("week", null, null);

            dto.TotalTicketsInPeriod.Should().Be(0);
            dto.StatusBreakdown.Should().BeEmpty();
            dto.AvgFirstResponseMinutes.Should().BeNull();
            dto.AvgRating.Should().BeNull();
            dto.AvgResolutionHours.Should().BeNull();
        }

        // ── US-72: Globalni vremenski filter — period parsing ─────────────────

        [Theory]
        [InlineData("week")]
        [InlineData("month")]
        [InlineData("year")]
        [InlineData("alltime")]
        public async Task GetAdminDashboardAsync_ShouldAcceptQuickPeriods(string period)
        {
            SetupRepoEmptyBaseline();
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Array.Empty<Ticket>());

            var act = async () => await _service.GetAdminDashboardAsync(period, null, null);
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldThrow_WhenCustomRangeReversed()
        {
            var act = () => _service.GetAdminDashboardAsync("custom", DateTime.UtcNow, DateTime.UtcNow.AddDays(-3));
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldThrow_WhenPeriodUnknown()
        {
            var act = () => _service.GetAdminDashboardAsync("nonsense", null, null);
            await act.Should().ThrowAsync<ArgumentException>();
        }

        // ── US-83: Generisanje izvještaja — sve podržane vrste ────────────────

        [Theory]
        [InlineData(ReportType.TICKET_COUNT)]
        [InlineData(ReportType.TICKET_STATUS)]
        [InlineData(ReportType.PROBLEM_TYPE)]
        [InlineData(ReportType.USER_RATINGS)]
        [InlineData(ReportType.FIRST_RESPONSE)]
        public async Task GenerateReportAsync_ShouldReturnDto_ForEachSupportedType(ReportType type)
        {
            SetupRepoEmptyBaseline();
            var now = DateTime.UtcNow;
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { MakeTicket(1, TicketStatus.OPEN, now.AddDays(-1), rating: 5) });

            var result = await _service.GenerateReportAsync(type, "month", null, null);

            result.ReportType.Should().Be(type.ToString());
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GenerateReportAsync_TeamWorkload_ShouldFetchAgentRows()
        {
            SetupRepoEmptyBaseline();
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { MakeTicket(1, TicketStatus.CLOSED, DateTime.UtcNow.AddDays(-2)) });
            _repo.Setup(r => r.GetAgentResolvedCountsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[]
                {
                    new AgentResolveRow { UserId = 2, FirstName = "Ag", LastName = "1", Role = Role.AGENT, ResolvedCount = 3 }
                });

            var result = await _service.GenerateReportAsync(ReportType.TEAM_WORKLOAD, "month", null, null);

            result.HasData.Should().BeTrue();
            var data = result.Data.Should().BeOfType<TeamWorkloadReportDto>().Subject;
            data.Items.Should().ContainSingle();
        }

        // ── US-83: Veliki period i TICKET_STATUS → warning ───────────────────

        [Fact]
        public async Task GenerateReportAsync_TicketStatus_ShouldSetLargePeriodWarning_ForCustomLargeRange()
        {
            SetupRepoEmptyBaseline();
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { MakeTicket(1, TicketStatus.OPEN, DateTime.UtcNow.AddMonths(-6)) });

            var from = DateTime.UtcNow.AddYears(-1);
            var to = DateTime.UtcNow;
            var result = await _service.GenerateReportAsync(ReportType.TICKET_STATUS, "custom", from, to);

            result.ShowLargePeriodWarning.Should().BeTrue();
        }

        // ── US-83: Prazan period → poruka ─────────────────────────────────────

        [Fact]
        public async Task GenerateReportAsync_ShouldReturnNoDataMessage_WhenEmptyTickets()
        {
            SetupRepoEmptyBaseline();
            _repo.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Array.Empty<Ticket>());

            var result = await _service.GenerateReportAsync(ReportType.PROBLEM_TYPE, "month", null, null);

            result.HasData.Should().BeFalse();
            result.Message.Should().Contain("Nema podataka");
        }
    }
}
