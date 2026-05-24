using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.Tests.Sprint9
{
    // PB-50 / US-87, US-88 — Integracijski test FIRST_RESPONSE preko
    // AdminController GET /api/admin/dashboard i ReportsController POST /api/reports/generate.
    public class FirstResponseReportIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static AdminController CreateAdminController(ApplicationDbContext context)
        {
            var controller = new AdminController(new ReportService(new ReportRepository(context)));
            SetClaims(controller, 1, "ADMINISTRATOR");
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

        private static void SeedTicketWithStaffReply(ApplicationDbContext context, int ticketId, int creatorId, int agentId, DateTime created, int firstResponseMinutes)
        {
            context.Tickets.Add(new Ticket
            {
                TicketId = ticketId,
                Title = $"T{ticketId}",
                Description = "D",
                CreatorId = creatorId,
                Status = TicketStatus.OPEN,
                Priority = Priority.MEDIUM,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = created,
            });
            context.Comments.Add(new Comment
            {
                CommentId = ticketId * 10,
                TicketId = ticketId,
                AuthorId = agentId,
                Content = "agent reply",
                DateTime = created.AddMinutes(firstResponseMinutes),
            });
        }

        // ── US-87: Dashboard KPI prikazuje aggregate u periodu ────────────────

        [Fact]
        public async Task GetDashboard_ShouldReturnAvgFirstResponse_OverCreatedTicketsInPeriod()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                new User { UserId = 1, FirstName = "Adm", LastName = "In", Email = "a@t", Username = "a", PasswordHash = "h", Role = Role.ADMINISTRATOR, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 2, FirstName = "C", LastName = "Lient", Email = "c@t", Username = "c", PasswordHash = "h", Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 3, FirstName = "Ag", LastName = "Ent", Email = "ag@t", Username = "ag", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE });

            var now = DateTime.UtcNow;
            SeedTicketWithStaffReply(context, 1, 2, 3, now.AddDays(-2), firstResponseMinutes: 30);
            SeedTicketWithStaffReply(context, 2, 2, 3, now.AddDays(-1), firstResponseMinutes: 50);
            await context.SaveChangesAsync();

            var controller = CreateAdminController(context);
            var result = await controller.GetDashboard("month", null, null);

            var dto = ((OkObjectResult)result).Value.Should().BeOfType<AdminDashboardDto>().Subject;
            dto.AvgFirstResponseMinutes.Should().NotBeNull();
            dto.AvgFirstResponseMinutes!.Value.Should().BeApproximately(40, 1);
        }

        [Fact]
        public async Task GetDashboard_ShouldReturnNullAvgFirstResponse_WhenNoStaffReplies()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                new User { UserId = 1, FirstName = "Adm", LastName = "In", Email = "a@t", Username = "a", PasswordHash = "h", Role = Role.ADMINISTRATOR, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 2, FirstName = "C", LastName = "Lient", Email = "c@t", Username = "c", PasswordHash = "h", Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE });
            context.Tickets.Add(new Ticket
            {
                TicketId = 99, Title = "no reply", Description = "D", CreatorId = 2,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
            });
            await context.SaveChangesAsync();

            var controller = CreateAdminController(context);
            var result = await controller.GetDashboard("month", null, null);

            var dto = ((OkObjectResult)result).Value.Should().BeOfType<AdminDashboardDto>().Subject;
            dto.AvgFirstResponseMinutes.Should().BeNull();
        }

        // ── US-88: Reports endpoint generates FIRST_RESPONSE report ────────────

        [Fact]
        public async Task GenerateReport_FirstResponse_ShouldReturnAvgAndBuckets()
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                new User { UserId = 1, FirstName = "C", LastName = "L", Email = "c@t", Username = "c", PasswordHash = "h", Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 3, FirstName = "Ag", LastName = "Ent", Email = "ag@t", Username = "ag", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE });

            var now = DateTime.UtcNow;
            SeedTicketWithStaffReply(context, 1, 1, 3, now.AddHours(-12), firstResponseMinutes: 10);
            SeedTicketWithStaffReply(context, 2, 1, 3, now.AddHours(-30), firstResponseMinutes: 20);
            await context.SaveChangesAsync();

            var controller = CreateReportsController(context);
            var result = await controller.GenerateReport(new ReportRequestDto { ReportType = ReportType.FIRST_RESPONSE, Period = "week" });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            report.HasData.Should().BeTrue();
            report.ReportType.Should().Be("FIRST_RESPONSE");

            var data = report.Data.Should().BeOfType<FirstResponseReportDto>().Subject;
            data.TicketsWithResponseCount.Should().Be(2);
            data.AvgFirstResponseMinutes.Should().BeApproximately(15, 1);
            data.BucketGranularityLabel.Should().Be("Po danu");
            data.Buckets.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GenerateReport_FirstResponse_ShouldReturnNoDataMessage_WhenEmpty()
        {
            using var context = CreateDbContext();
            var controller = CreateReportsController(context);

            var result = await controller.GenerateReport(new ReportRequestDto { ReportType = ReportType.FIRST_RESPONSE, Period = "week" });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            report.HasData.Should().BeFalse();
            report.Message.Should().Contain("Nema podataka");
        }

        [Theory]
        [InlineData("week", "Po danu")]
        [InlineData("month", "Po sedmici")]
        [InlineData("year", "Po mjesecu")]
        public async Task GenerateReport_FirstResponse_ShouldUseExpectedGranularityLabel(string period, string expectedLabel)
        {
            using var context = CreateDbContext();
            context.Users.AddRange(
                new User { UserId = 1, FirstName = "C", LastName = "L", Email = "c@t", Username = "c", PasswordHash = "h", Role = Role.CLIENT, AccountStatus = AccountStatus.ACTIVE },
                new User { UserId = 3, FirstName = "Ag", LastName = "E", Email = "ag@t", Username = "ag", PasswordHash = "h", Role = Role.AGENT, AccountStatus = AccountStatus.ACTIVE });
            SeedTicketWithStaffReply(context, 1, 1, 3, DateTime.UtcNow.AddHours(-1), 20);
            await context.SaveChangesAsync();

            var controller = CreateReportsController(context);
            var result = await controller.GenerateReport(new ReportRequestDto { ReportType = ReportType.FIRST_RESPONSE, Period = period });

            var report = ((OkObjectResult)result).Value.Should().BeOfType<ReportResultDto>().Subject;
            var data = report.Data.Should().BeOfType<FirstResponseReportDto>().Subject;
            data.BucketGranularityLabel.Should().Be(expectedLabel);
        }
    }
}
