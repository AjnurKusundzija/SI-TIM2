using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.AuditLog;

public class AuditLogServiceTests
{
    [Fact]
    public async Task LogAsync_CreatesAuditLogRecord_WithCorrectTimestamp()
    {
        await using var context = CreateContext();
        var service = new AuditLogService(context, Mock.Of<ILogger<AuditLogService>>());
        var before = DateTime.UtcNow;

        await service.LogAsync(AuditActionType.USER_LOGIN, "User", "1", "Test login", userId: 1);

        var after = DateTime.UtcNow;
        var log = await context.AuditLogs.SingleAsync();
        log.Timestamp.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        log.ActionType.Should().Be(nameof(AuditActionType.USER_LOGIN));
        log.EntityType.Should().Be("User");
        log.EntityId.Should().Be("1");
        log.Description.Should().Be("Test login");
        log.UserId.Should().Be(1);
    }

    [Fact]
    public async Task LogAsync_DoesNotSerializePasswordOrHashFields()
    {
        await using var context = CreateContext();
        var service = new AuditLogService(context, Mock.Of<ILogger<AuditLogService>>());

        await service.LogAsync(
            AuditActionType.USER_CREATED,
            "User",
            "5",
            "User created",
            newValue: new { firstName = "John", passwordHash = "tajna", nested = new { token = "hidden", email = "john@example.com" } });

        var log = await context.AuditLogs.SingleAsync();
        log.NewValue.Should().NotContain("tajna");
        log.NewValue.Should().NotContain("passwordHash");
        log.NewValue.Should().NotContain("hidden");
        log.NewValue.Should().Contain("John");
        log.NewValue.Should().Contain("john@example.com");
    }

    [Fact]
    public async Task LogAsync_DoesNotPropagateException()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var contextMock = new Mock<ApplicationDbContext>(options);
        var loggerMock = new Mock<ILogger<AuditLogService>>();
        contextMock
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));
        var service = new AuditLogService(contextMock.Object, loggerMock.Object);

        var act = () => service.LogAsync(AuditActionType.USER_LOGIN, "User", "1", "Test", userId: 1);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task TicketService_CreateTicket_LogsTicketCreated()
    {
        var ticketRepository = new Mock<ITicketRepository>();
        var teamRepository = new Mock<ITeamRepository>();
        var userRepository = new Mock<IUserRepository>();
        var auditLogService = new Mock<IAuditLogService>();

        teamRepository
            .Setup(r => r.GetBySpecializedCategoryAsync(ProblemCategory.INTERNET))
            .ReturnsAsync((Team?)null);
        ticketRepository
            .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(ticket => ticket.TicketId = 42)
            .ReturnsAsync((Ticket ticket) => ticket);

        var service = new TicketService(
            ticketRepository.Object,
            teamRepository.Object,
            userRepository.Object,
            Mock.Of<INotificationService>(),
            new TelecomSupportSystem.DAL.Repositories.NullAttachmentRepository(),
            Mock.Of<ICommentService>(),
            auditLogService.Object);

        await service.CreateTicketAsync(new CreateTicketDto
        {
            Subject = "Internet ne radi",
            Description = "Opis problema",
            Type = ProblemCategory.INTERNET,
            Priority = Priority.HIGH
        }, userId: 7);

        auditLogService.Verify(a => a.LogAsync(
            AuditActionType.TICKET_CREATED,
            "Ticket",
            "42",
            It.IsAny<string>(),
            7,
            null,
            It.IsAny<object>(),
            null), Times.Once);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }
}
