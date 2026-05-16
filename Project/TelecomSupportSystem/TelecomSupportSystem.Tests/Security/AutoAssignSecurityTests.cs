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

namespace TelecomSupportSystem.Tests.Security
{
    public class AutoAssignSecurityTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static TicketController CreateController(ApplicationDbContext context, int userId)
        {
            var controller = new TicketController(new TicketService(
                new TicketRepository(context),
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object, new Mock<ICommentService>().Object));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
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

        [Fact]
        public async Task CreateTicket_DoesNotAssignToTechnicianAdministratorOrClient_EvenWhenTheyAreAvailableInTeam()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam());
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.TECHNICIAN, teamId: 10, status: AvailabilityStatus.AVAILABLE),
                MakeUser(3, Role.ADMINISTRATOR, teamId: 10, status: AvailabilityStatus.AVAILABLE),
                MakeUser(4, Role.CLIENT, teamId: 10, status: AvailabilityStatus.AVAILABLE),
                MakeUser(5, Role.AGENT, teamId: 10, status: AvailabilityStatus.AVAILABLE));
            await context.SaveChangesAsync();

            var result = await CreateController(context, 1).CreateTicket(new CreateTicketDto
            {
                Subject = "Internet",
                Description = "Problem",
                Priority = Priority.MEDIUM,
                Type = ProblemCategory.INTERNET
            });

            result.Should().BeOfType<CreatedAtActionResult>();

            var assignment = context.Set<TicketUser>().Should().ContainSingle().Subject;
            assignment.UserId.Should().Be(5);
            assignment.AssignmentType.Should().Be(AssignmentType.AUTOMATIC);
        }

        [Fact]
        public async Task CreateTicket_WhenOnlyNonAgentUsersMatchTeam_ReturnsNoAvailableAgentsAndNoAssignment()
        {
            using var context = CreateDbContext();
            context.Teams.Add(MakeTeam());
            context.Users.AddRange(
                MakeUser(1, Role.CLIENT),
                MakeUser(2, Role.TECHNICIAN, teamId: 10, status: AvailabilityStatus.AVAILABLE),
                MakeUser(3, Role.ADMINISTRATOR, teamId: 10, status: AvailabilityStatus.AVAILABLE),
                MakeUser(4, Role.CLIENT, teamId: 10, status: AvailabilityStatus.AVAILABLE));
            await context.SaveChangesAsync();

            var result = await CreateController(context, 1).CreateTicket(new CreateTicketDto
            {
                Subject = "Internet",
                Description = "Problem",
                Priority = Priority.MEDIUM,
                Type = ProblemCategory.INTERNET
            });

            var dto = result.Should().BeOfType<CreatedAtActionResult>().Subject.Value
                .Should().BeAssignableTo<GetTicketDto>().Subject;

            dto.AssignedAgentName.Should().BeNull();
            dto.AssignmentMessage.Should().StartWith("Nema dostupnih agenata");
            context.Set<TicketUser>().Should().BeEmpty();
        }
    }
}
