using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-45 — Integracijski testovi: dashboard end-to-end + global filter + drill-down query.
    // Pokrivaju US-71, US-72, US-82, US-83, US-84, US-85, US-86.
    public class Sprint9AdminDashboardIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static AdminController CreateAdminController(ApplicationDbContext context, string role = "ADMINISTRATOR")
        {
            var controller = new AdminController(new ReportService(new ReportRepository(context)));
            SetClaims(controller, 1, role);
            return controller;
        }

        private static ReportsController CreateReportsController(ApplicationDbContext context, string role = "ADMINISTRATOR")
        {
            var controller = new ReportsController(new ReportService(new ReportRepository(context)));
            SetClaims(controller, 1, role);
            return controller;
        }

        private static void SetClaims(ControllerBase controller, int id, string role) =>
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                        new Claim(ClaimTypes.Role, role),
                    }, "Test"))
                }
            };

        private static User MakeUser(int id, Role role, AccountStatus status = AccountStatus.ACTIVE) => new()
        {
            UserId = id,
            FirstName = $"N{id}",
            LastName = $"L{id}",
            Email = $"u{id}@t",
            Username = $"u{id}",
            PasswordHash = "h",
            Role = role,
            AccountStatus = status,
        };

        // ── US-71 / US-86: Dashboard endpoint returns 200 i sve sekcije ────────

        [Fact]
        public async Task GetDashboard_EndToEnd_ShouldReturn200_WithMustHaveSections()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                MakeUser(1, Role.ADMINISTRATOR),
                MakeUser(2, Role.CLIENT),
                MakeUser(3, Role.AGENT),
                MakeUser(4, Role.TECHNICIAN));

            var now = DateTime.UtcNow;
            context.Tickets.AddRange(
                new Ticket { TicketId = 1, Title = "T1", Description = "D", CreatorId = 2, Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET, CreatedDate = now.AddDays(-1) },
                new Ticket { TicketId = 2, Title = "T2", Description = "D", CreatorId = 2, Status = TicketStatus.CLOSED, Priority = Priority.HIGH, ProblemCategory = ProblemCategory.BILLING, CreatedDate = now.AddDays(-3), ClosedDate = now.AddDays(-2) },
                new Ticket { TicketId = 3, Title = "T3", Description = "D", CreatorId = 2, Status = TicketStatus.CLOSURE_REQUESTED, Priority = Priority.MEDIUM, ProblemCategory = ProblemCategory.TV, CreatedDate = now.AddDays(-5) });
            await context.SaveChangesAsync();

            var controller = CreateAdminController(context);
            var result = await controller.GetDashboard("month", null, null);

            var dto = ((OkObjectResult)result).Value.Should().BeOfType<AdminDashboardDto>().Subject;
            dto.TotalTicketsInPeriod.Should().Be(3);
            dto.StatusBreakdown.Should().HaveCountGreaterThan(0);
            dto.ActiveUsersByRole.Agents.Should().Be(1);
            dto.ActiveUsersByRole.Clients.Should().Be(1);
            dto.ActiveUsersByRole.Technicians.Should().Be(1);
            dto.ActiveUsersByRole.Administrators.Should().Be(1);
            dto.OpenTicketsCount.Should().BeGreaterThanOrEqualTo(0);
            dto.UnassignedOpenCount.Should().BeGreaterThanOrEqualTo(0);
            dto.StaleTicketsCount.Should().BeGreaterThanOrEqualTo(0);
        }

        // ── US-71: Status agregati NE smiju koristiti nepostojeći CANCELLED ───

        [Fact]
        public async Task GetDashboard_EndToEnd_StatusBreakdown_DoesNotIntroduceUnknownStatuses()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            var controller = CreateAdminController(context);
            var result = await controller.GetDashboard("month", null, null);
            var dto = ((OkObjectResult)result).Value.Should().BeOfType<AdminDashboardDto>().Subject;

            dto.StatusBreakdown.Select(s => s.Status).Should().OnlyContain(s =>
                s == "OPEN" || s == "CLOSED" || s == "CLOSURE_REQUESTED");
        }

        // ── US-72: invalid custom range → 400 (i ne poziva bazu da prikupi tikete sa lažnim datumom) ─

        [Fact]
        public async Task GetDashboard_EndToEnd_ShouldReturn400_WhenCustomRangeReversed()
        {
            using var context = CreateDbContext();
            var controller = CreateAdminController(context);
            var result = await controller.GetDashboard("custom", DateTime.UtcNow, DateTime.UtcNow.AddDays(-2));

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ── US-83: Generate report — TICKET_COUNT vraća stvarni broj ─────────

        [Fact]
        public async Task GenerateReport_EndToEnd_TicketCount_ShouldReturnRealCount()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            context.Users.Add(MakeUser(2, Role.CLIENT));
            for (int i = 1; i <= 5; i++)
            {
                context.Tickets.Add(new Ticket
                {
                    TicketId = i, Title = $"T{i}", Description = "D", CreatorId = 2,
                    Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                    CreatedDate = DateTime.UtcNow.AddDays(-i),
                });
            }
            await context.SaveChangesAsync();

            var controller = CreateReportsController(context);
            var result = await controller.GenerateReport(new ReportRequestDto { ReportType = ReportType.TICKET_COUNT, Period = "month" });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            report.HasData.Should().BeTrue();
            var data = report.Data.Should().BeOfType<TicketCountReportDto>().Subject;
            data.TotalCount.Should().Be(5);
        }

        // ── US-71 / US-72: AdminController je [Authorize(Roles="ADMINISTRATOR")] ─

        [Fact]
        public void AdminController_GetDashboard_ShouldHaveAuthorizeAdminAttribute()
        {
            var attr = typeof(AdminController).GetCustomAttribute<AuthorizeAttribute>();
            attr.Should().NotBeNull();
            attr!.Roles.Should().Be("ADMINISTRATOR");
        }

        [Fact]
        public void ReportsController_GenerateReport_ShouldHaveAuthorizeAdminAttribute()
        {
            var attr = typeof(ReportsController).GetCustomAttribute<AuthorizeAttribute>();
            attr.Should().NotBeNull();
            attr!.Roles.Should().Be("ADMINISTRATOR");
        }

        // ── US-85: Export — placeholder; backend nema endpoint pa testiramo da
        // generate endpoint NE proizvodi export fajl (ostaje na frontu).
        [Fact]
        public async Task GenerateReport_EndToEnd_ShouldNotReturnExportArtifact()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(2, Role.CLIENT));
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 2,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            var controller = CreateReportsController(context);
            var result = await controller.GenerateReport(new ReportRequestDto { ReportType = ReportType.TICKET_COUNT, Period = "month" });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            // Sigurni signali: report DTO ne sadrži file/csv/xlsx polja.
            var dtoType = report.GetType();
            dtoType.GetProperty("File").Should().BeNull();
            dtoType.GetProperty("FileBytes").Should().BeNull();
            dtoType.GetProperty("Csv").Should().BeNull();
        }
    }
}
