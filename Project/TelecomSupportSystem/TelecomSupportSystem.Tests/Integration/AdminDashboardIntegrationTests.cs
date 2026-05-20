using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Integration
{
    public class AdminDashboardIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static AdminController CreateController(ApplicationDbContext context)
        {
            var service = new ReportService(new ReportRepository(context));
            var controller = new AdminController(service);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ClaimTypes.Role, "ADMINISTRATOR"),
                    }, "Test")),
                },
            };
            return controller;
        }

        private static User MakeUser(int id, Role role) => new()
        {
            UserId = id,
            FirstName = "Test",
            LastName = "User",
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = role,
        };

        [Fact]
        public async Task GetDashboard_ShouldReturn200_WithTicketCounts()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            context.Tickets.Add(new Ticket
            {
                Title = "T1",
                Description = "d",
                CreatorId = 1,
                Status = TicketStatus.OPEN,
                Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            var controller = CreateController(context);
            var result = await controller.GetDashboard("month", null, null);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeAssignableTo<TelecomSupportSystem.BLL.DTOs.Reports.AdminDashboardDto>().Subject;
            dto.TotalTicketsInPeriod.Should().Be(1);
            dto.OpenTicketsCount.Should().Be(1);
        }

        [Fact]
        public async Task GetDashboard_ShouldReturn400_WhenCustomRangeInvalid()
        {
            using var context = CreateDbContext();
            var controller = CreateController(context);

            var result = await controller.GetDashboard("custom", DateTime.UtcNow, DateTime.UtcNow.AddDays(-2));

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
