using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    /// <summary>
    /// PB-45 / PB-50 — Admin dashboard and report aggregation tests.
    /// </summary>
    public class AdminReportServiceTests
    {
        private readonly Mock<IReportRepository> _repoMock = new();
        private readonly ReportService _service;

        public AdminReportServiceTests()
        {
            _service = new ReportService(_repoMock.Object);
        }

        private static Ticket MakeTicket(
            int id,
            TicketStatus status,
            DateTime created,
            ProblemCategory category = ProblemCategory.INTERNET,
            int? rating = null)
        {
            var ticket = new Ticket
            {
                TicketId = id,
                Title = $"T{id}",
                CreatorId = 1,
                Status = status,
                ProblemCategory = category,
                CreatedDate = created,
                ClosedDate = status == TicketStatus.CLOSED ? created.AddHours(2) : null,
                Comments = new List<Comment>(),
                Rating = rating.HasValue
                    ? new DAL.Entities.Rating { RatingValue = rating.Value, UserId = 1, TicketId = id }
                    : null,
            };

            ticket.Comments.Add(new Comment
            {
                CommentId = id,
                TicketId = id,
                AuthorId = 2,
                DateTime = created.AddMinutes(30),
                Content = "Odgovor",
                Author = new User { UserId = 2, Role = Role.AGENT },
            });

            return ticket;
        }

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldReturnStatusBreakdownAndFirstResponse_WhenTicketsExist()
        {
            var now = DateTime.UtcNow;
            var tickets = new[]
            {
                MakeTicket(1, TicketStatus.OPEN, now.AddDays(-1)),
                MakeTicket(2, TicketStatus.CLOSED, now.AddDays(-2), rating: 5),
            };

            _repoMock.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(tickets);
            _repoMock.Setup(r => r.GetActiveUserCountsByRoleAsync())
                .ReturnsAsync(new UserRoleCounts { Agents = 2, Clients = 5 });
            _repoMock.Setup(r => r.GetAgentResolvedCountsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Array.Empty<AgentResolveRow>());
            _repoMock.Setup(r => r.GetOpenTicketsCountAsync()).ReturnsAsync(1);
            _repoMock.Setup(r => r.GetClosureRequestedCountAsync()).ReturnsAsync(0);
            _repoMock.Setup(r => r.GetUnassignedOpenTicketsCountAsync()).ReturnsAsync(0);
            _repoMock.Setup(r => r.GetStaleTicketsCountAsync(It.IsAny<DateTime>())).ReturnsAsync(0);

            var result = await _service.GetAdminDashboardAsync("month", null, null);

            result.TotalTicketsInPeriod.Should().Be(2);
            result.StatusBreakdown.Should().HaveCount(2);
            result.AvgFirstResponseMinutes.Should().BeApproximately(30, 0.1);
            result.AvgRating.Should().Be(5);
            result.ActiveUsersByRole.Agents.Should().Be(2);
        }

        [Fact]
        public async Task GetAdminDashboardAsync_ShouldThrow_WhenCustomRangeInvalid()
        {
            var act = () => _service.GetAdminDashboardAsync("custom", DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GenerateReportAsync_ShouldReturnNoDataMessage_WhenEmpty()
        {
            _repoMock.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(Array.Empty<Ticket>());

            var result = await _service.GenerateReportAsync(ReportType.TICKET_COUNT, "week", null, null);

            result.HasData.Should().BeFalse();
            result.Message.Should().Contain("Nema podataka");
        }

        [Fact]
        public async Task GenerateReportAsync_ShouldReturnFirstResponseBuckets_WhenRequested()
        {
            var now = DateTime.UtcNow;
            var tickets = Enumerable.Range(1, 5).Select(i =>
                MakeTicket(i, TicketStatus.OPEN, now.AddDays(-i))).ToArray();

            _repoMock.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(tickets);

            var result = await _service.GenerateReportAsync(ReportType.FIRST_RESPONSE, "week", null, null);

            result.HasData.Should().BeTrue();
            var data = result.Data.Should().BeOfType<TelecomSupportSystem.BLL.DTOs.Reports.FirstResponseReportDto>().Subject;
            data.TicketsWithResponseCount.Should().Be(5);
            data.Buckets.Should().NotBeEmpty();
            data.BucketGranularityLabel.Should().Be("Po danu");
            data.AvgFirstResponseMinutes.Should().BeApproximately(30, 0.1);
        }

        [Fact]
        public async Task GenerateReportAsync_ShouldWarnOnLargePeriod_ForStatusReport()
        {
            var ticket = MakeTicket(1, TicketStatus.OPEN, DateTime.UtcNow.AddMonths(-6));
            _repoMock.Setup(r => r.GetTicketsCreatedInPeriodAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new[] { ticket });

            var from = DateTime.UtcNow.AddYears(-1);
            var result = await _service.GenerateReportAsync(ReportType.TICKET_STATUS, "custom", from, DateTime.UtcNow);

            result.ShowLargePeriodWarning.Should().BeTrue();
            result.HasData.Should().BeTrue();
        }
    }
}
