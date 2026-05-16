using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.DataIntegrity
{
    public class AutoAssignDataIntegrityTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context)
        {
            var controller = new TicketController(new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "CLIENT")
                    }, "Test"))
                }
            };

            return controller;
        }

        private static User MakeUser(int id, Role role, int? teamId = null, AvailabilityStatus? status = null) => new()
        {
            UserId = id,
            FirstName = $"{role}{id}",
            LastName = "Test",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            Role = role,
            AccountStatus = AccountStatus.ACTIVE,
            TeamId = teamId,
            AvailabilityStatus = status
        };

        private static Team MakeTeam() => new()
        {
            TeamId = 10,
            TeamName = "Internet Team",
            TeamType = TeamType.AGENTS,
            TeamStatus = TeamStatus.ACTIVE,
            SpecializedCategory = ProblemCategory.INTERNET
        };

        private static CreateTicketDto MakeDto() => new()
        {
            Subject = "Internet problem",
            Description = "Opis",
            Priority = Priority.MEDIUM,
            Type = ProblemCategory.INTERNET
        };

        [Fact]
        public async Task CreateTicket_WhenAssignmentFails_PersistsTicketButDoesNotCreateOrphanTicketUser()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam());
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.AGENT, teamId: 10, status: AvailabilityStatus.BUSY));
            await context.SaveChangesAsync();

            var result = await CreateController(context).CreateTicket(MakeDto());

            result.Should().BeOfType<CreatedAtActionResult>();
            context.Tickets.Should().ContainSingle(t => t.CreatorId == 1);
            context.Set<TicketUser>().Should().BeEmpty();
        }

        [Fact]
        public async Task CreateTicket_WhenAssignmentSucceeds_TicketUserReferencesExistingTicketUserAndTeam()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam());
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.AGENT, teamId: 10, status: AvailabilityStatus.AVAILABLE));
            await context.SaveChangesAsync();

            var result = await CreateController(context).CreateTicket(MakeDto());
            var dto = result.Should().BeOfType<CreatedAtActionResult>().Subject.Value
                .Should().BeAssignableTo<GetTicketDto>().Subject;

            var assignment = context.Set<TicketUser>().Should().ContainSingle().Subject;
            assignment.TicketId.Should().Be(dto.TicketId);
            assignment.UserId.Should().Be(2);
            assignment.TeamId.Should().Be(10);
            (await context.Tickets.AnyAsync(t => t.TicketId == assignment.TicketId)).Should().BeTrue();
            (await context.Users.AnyAsync(u => u.UserId == assignment.UserId)).Should().BeTrue();
            (await context.Teams.AnyAsync(t => t.TeamId == assignment.TeamId)).Should().BeTrue();
        }
    }
}
