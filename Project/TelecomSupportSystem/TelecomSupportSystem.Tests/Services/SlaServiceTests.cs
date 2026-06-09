using Xunit;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.Tests.Services;

/// <summary>
/// Unit tests for SlaService covering PB-65 (US-115, US-116).
/// </summary>
public class SlaServiceTests
{
    private readonly SlaService _sut = new();

    private static Ticket MakeTicket(Priority priority, DateTime createdDate, TicketStatus status = TicketStatus.OPEN) => new()
    {
        TicketId = 1,
        Title = "Test tiket",
        Priority = priority,
        CreatedDate = createdDate,
        Status = status,
    };

    // ─── GetSlaInfo — rokovi po prioritetu (US-115) ───────────────────────────

    // US-115: CRITICAL rok je 2 sata
    [Fact]
    public void GetSlaInfo_CriticalPriority_DeadlineIs2HoursFromCreation()
    {
        var created = DateTime.UtcNow.AddHours(-1);
        var ticket = MakeTicket(Priority.CRITICAL, created);

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal(created.AddHours(2), result.Deadline, TimeSpan.FromSeconds(1));
    }

    // US-115: HIGH rok je 8 sati
    [Fact]
    public void GetSlaInfo_HighPriority_DeadlineIs8HoursFromCreation()
    {
        var created = DateTime.UtcNow.AddHours(-1);
        var ticket = MakeTicket(Priority.HIGH, created);

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal(created.AddHours(8), result.Deadline, TimeSpan.FromSeconds(1));
    }

    // US-115: MEDIUM rok je 24 sata
    [Fact]
    public void GetSlaInfo_MediumPriority_DeadlineIs24HoursFromCreation()
    {
        var created = DateTime.UtcNow.AddHours(-1);
        var ticket = MakeTicket(Priority.MEDIUM, created);

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal(created.AddHours(24), result.Deadline, TimeSpan.FromSeconds(1));
    }

    // US-115: LOW rok je 72 sata
    [Fact]
    public void GetSlaInfo_LowPriority_DeadlineIs72HoursFromCreation()
    {
        var created = DateTime.UtcNow.AddHours(-1);
        var ticket = MakeTicket(Priority.LOW, created);

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal(created.AddHours(72), result.Deadline, TimeSpan.FromSeconds(1));
    }

    // ─── GetSlaInfo — boja-kodiranje statusa (US-115) ─────────────────────────

    // US-115: > 50% preostalog → GREEN
    [Fact]
    public void GetSlaInfo_MoreThan50PercentRemaining_ReturnsGreen()
    {
        // MEDIUM = 24h; kreirano 5h ago → 19/24 = 79% preostalo
        var ticket = MakeTicket(Priority.MEDIUM, DateTime.UtcNow.AddHours(-5));

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal("GREEN", result.Status);
        Assert.False(result.IsBreached);
    }

    // US-115: 20–50% preostalog → YELLOW
    [Fact]
    public void GetSlaInfo_Between20And50PercentRemaining_ReturnsYellow()
    {
        // MEDIUM = 24h; kreirano 16h ago → 8/24 = 33% preostalo
        var ticket = MakeTicket(Priority.MEDIUM, DateTime.UtcNow.AddHours(-16));

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal("YELLOW", result.Status);
        Assert.False(result.IsBreached);
    }

    // US-115: < 20% preostalog → RED (bez breasha)
    [Fact]
    public void GetSlaInfo_LessThan20PercentRemaining_ReturnsRed()
    {
        // MEDIUM = 24h; kreirano 22h ago → 2/24 = 8% preostalo
        var ticket = MakeTicket(Priority.MEDIUM, DateTime.UtcNow.AddHours(-22));

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal("RED", result.Status);
        Assert.False(result.IsBreached);
    }

    // US-115: rok prekoračen → RED + IsBreached = true
    [Fact]
    public void GetSlaInfo_DeadlinePassed_ReturnsRedAndIsBreachedTrue()
    {
        // CRITICAL = 2h; kreirano 3h ago → prekoračeno
        var ticket = MakeTicket(Priority.CRITICAL, DateTime.UtcNow.AddHours(-3));

        var result = _sut.GetSlaInfo(ticket);

        Assert.Equal("RED", result.Status);
        Assert.True(result.IsBreached);
        Assert.True(result.RemainingMinutes < 0);
        Assert.Equal(0, result.PercentRemaining);
    }

    // ─── CountBreaches (US-116) ───────────────────────────────────────────────

    // US-116: zatvoreni tiketi sa prekoračenim SLA se ne broje
    [Fact]
    public void CountBreaches_ClosedTicketWithBreachedSla_NotCounted()
    {
        var closedBreached = MakeTicket(Priority.CRITICAL, DateTime.UtcNow.AddHours(-5), TicketStatus.CLOSED);

        var count = _sut.CountBreaches(new[] { closedBreached });

        Assert.Equal(0, count);
    }

    // US-116: otvoreni tiket s prekoračenim SLA se broji
    [Fact]
    public void CountBreaches_OpenTicketWithBreachedSla_CountedOnce()
    {
        var openBreached = MakeTicket(Priority.CRITICAL, DateTime.UtcNow.AddHours(-5));

        var count = _sut.CountBreaches(new[] { openBreached });

        Assert.Equal(1, count);
    }

    // US-116: mješavina tiketa — broji se samo otvoreni s breachom
    [Fact]
    public void CountBreaches_MixedTickets_CountsOnlyOpenBreached()
    {
        var tickets = new[]
        {
            MakeTicket(Priority.CRITICAL, DateTime.UtcNow.AddHours(-5)),              // open + breached
            MakeTicket(Priority.CRITICAL, DateTime.UtcNow.AddHours(-5), TicketStatus.CLOSED), // closed + breached
            MakeTicket(Priority.MEDIUM,   DateTime.UtcNow.AddHours(-1)),              // open + not breached
        };

        var count = _sut.CountBreaches(tickets);

        Assert.Equal(1, count);
    }

    // US-116: prazna lista tiketa vraća 0
    [Fact]
    public void CountBreaches_EmptyList_ReturnsZero()
    {
        var count = _sut.CountBreaches(Enumerable.Empty<Ticket>());

        Assert.Equal(0, count);
    }
}
