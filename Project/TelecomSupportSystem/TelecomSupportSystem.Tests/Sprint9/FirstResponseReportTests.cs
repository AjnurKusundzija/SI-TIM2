using FluentAssertions;
using TelecomSupportSystem.BLL.Helpers;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-50 / US-87, US-88 — Prosjecno vrijeme prvog odgovora
    // Unit testovi helper logike: TicketMetricsHelper, FirstResponseReportHelper.
    public class FirstResponseReportTests
    {
        private static Ticket MakeTicket(int id, DateTime created, params (int authorId, Role role, DateTime when)[] comments)
        {
            var ticket = new Ticket
            {
                TicketId = id,
                Title = $"T{id}",
                CreatorId = 1,
                Description = "D",
                Status = TicketStatus.OPEN,
                Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = created,
                Comments = new List<Comment>(),
            };
            foreach (var (authorId, role, when) in comments)
            {
                ticket.Comments.Add(new Comment
                {
                    CommentId = ticket.Comments.Count + 1,
                    TicketId = id,
                    AuthorId = authorId,
                    Author = new User { UserId = authorId, Role = role, Email = $"u{authorId}@t", FirstName = "F", LastName = "L", Username = $"u{authorId}", PasswordHash = "h" },
                    Content = "msg",
                    DateTime = when,
                });
            }
            return ticket;
        }

        // ── US-87: aggregate uses staff first response, NOT client ─────────────

        [Fact]
        public void GetFirstResponseMinutes_ShouldUseFirstStaffComment_NotClient()
        {
            var created = DateTime.UtcNow.AddHours(-3);
            var ticket = MakeTicket(1, created,
                (authorId: 1, Role.CLIENT, created.AddMinutes(10)),
                (authorId: 2, Role.AGENT, created.AddMinutes(30)),
                (authorId: 3, Role.TECHNICIAN, created.AddMinutes(45)));

            var minutes = TicketMetricsHelper.GetFirstResponseMinutes(ticket);

            minutes.Should().NotBeNull();
            minutes!.Value.Should().BeApproximately(30, 0.5);
        }

        [Fact]
        public void GetFirstResponseMinutes_ShouldReturnNull_WhenOnlyClientComments()
        {
            var created = DateTime.UtcNow.AddHours(-2);
            var ticket = MakeTicket(1, created, (1, Role.CLIENT, created.AddMinutes(5)));

            var minutes = TicketMetricsHelper.GetFirstResponseMinutes(ticket);

            minutes.Should().BeNull();
        }

        [Fact]
        public void CalculateAvgFirstResponseMinutes_ShouldIgnoreTicketsWithoutStaffComments()
        {
            var now = DateTime.UtcNow;
            var t1 = MakeTicket(1, now.AddHours(-2), (2, Role.AGENT, now.AddHours(-2).AddMinutes(20)));
            var t2 = MakeTicket(2, now.AddHours(-3)); // no comments
            var t3 = MakeTicket(3, now.AddHours(-4), (1, Role.CLIENT, now.AddHours(-4).AddMinutes(10))); // only client

            var avg = TicketMetricsHelper.CalculateAvgFirstResponseMinutes(new[] { t1, t2, t3 });

            avg.Should().NotBeNull();
            avg!.Value.Should().BeApproximately(20, 0.5);
        }

        [Fact]
        public void CalculateAvgFirstResponseMinutes_ShouldReturnNull_WhenNoTicketHasStaffResponse()
        {
            var now = DateTime.UtcNow;
            var t1 = MakeTicket(1, now.AddHours(-2));
            var t2 = MakeTicket(2, now.AddHours(-1), (1, Role.CLIENT, now.AddMinutes(-30)));

            var avg = TicketMetricsHelper.CalculateAvgFirstResponseMinutes(new[] { t1, t2 });

            avg.Should().BeNull();
        }

        // ── US-88: bucket granularity rules ────────────────────────────────────

        [Theory]
        [InlineData("week", ReportBucketGranularity.Day, "Po danu")]
        [InlineData("month", ReportBucketGranularity.Week, "Po sedmici")]
        [InlineData("year", ReportBucketGranularity.Month, "Po mjesecu")]
        public void ResolveGranularity_ShouldMapStandardPeriods(string period, ReportBucketGranularity expected, string label)
        {
            var (g, l) = FirstResponseReportHelper.ResolveGranularity(period, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
            g.Should().Be(expected);
            l.Should().Be(label);
        }

        [Theory]
        [InlineData(7, ReportBucketGranularity.Day)]   // ≤14
        [InlineData(60, ReportBucketGranularity.Week)] // ≤90
        [InlineData(200, ReportBucketGranularity.Month)] // >90
        public void ResolveGranularity_Custom_ShouldPickByRangeWidth(int days, ReportBucketGranularity expected)
        {
            var to = DateTime.UtcNow;
            var from = to.AddDays(-days);
            var (g, _) = FirstResponseReportHelper.ResolveGranularity("custom", from, to);
            g.Should().Be(expected);
        }

        [Fact]
        public void Build_ShouldReturnZero_WhenNoTicketsForPeriod()
        {
            var now = DateTime.UtcNow;
            var result = FirstResponseReportHelper.Build(new List<Ticket>(), "month", now.AddDays(-30), now);

            result.TotalTicketsCount.Should().Be(0);
            result.TicketsWithResponseCount.Should().Be(0);
            result.AvgFirstResponseMinutes.Should().BeNull();
            result.Buckets.Should().NotBeNull();
        }

        [Fact]
        public void Build_ShouldGroupTicketsIntoBuckets_AndComputeAvgPerBucket()
        {
            var anchor = DateTime.SpecifyKind(new DateTime(2026, 2, 10), DateTimeKind.Utc);
            var t1 = MakeTicket(1, anchor.AddDays(1), (2, Role.AGENT, anchor.AddDays(1).AddMinutes(20)));
            var t2 = MakeTicket(2, anchor.AddDays(2), (2, Role.AGENT, anchor.AddDays(2).AddMinutes(60)));

            var result = FirstResponseReportHelper.Build(new List<Ticket> { t1, t2 }, "week", anchor, anchor.AddDays(7));

            result.Buckets.Should().NotBeEmpty();
            result.TicketsWithResponseCount.Should().Be(2);
            result.AvgFirstResponseMinutes.Should().BeApproximately(40, 0.5);
            result.BucketGranularityLabel.Should().Be("Po danu");
        }
    }
}
