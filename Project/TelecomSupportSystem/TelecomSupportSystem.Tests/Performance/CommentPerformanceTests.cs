using System.Diagnostics;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Performance
{
    // PB-27: Performansno testiranje dohvata i slanja komentara
    public class CommentPerformanceTests
    {
        private const int MaxLoadTimeMilliseconds = 2000;

        private static IHubContext<ChatHub> BuildHubStub()
        {
            var hubMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var proxyMock = new Mock<IClientProxy>();
            hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(proxyMock.Object);
            proxyMock.Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            return hubMock.Object;
        }

        // US-15: lista od 100 komentara se ucitava u prihvatljivom vremenu
        [Fact]
        public async Task GetComments_ShouldLoadLargeListWithinTimeLimit_InTestEnvironment()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var client = new User
            {
                UserId = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "t@test.ba",
                Username = "tuser",
                PasswordHash = "hash",
                AccountStatus = AccountStatus.ACTIVE,
                Role = Role.CLIENT,
            };
            context.Users.Add(client);
            var ticket = new Ticket
            {
                TicketId = 1,
                Title = "Tiket",
                Description = "Opis.",
                CreatorId = 1,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.HIGH,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>(),
            };
            context.Tickets.Add(ticket);
            for (var i = 1; i <= 100; i++)
            {
                context.Comments.Add(new Comment
                {
                    CommentId = i,
                    TicketId = 1,
                    AuthorId = 1,
                    Author = client,
                    Content = $"Komentar {i}",
                    DateTime = DateTime.UtcNow.AddMinutes(-i),
                    IsInternal = false,
                });
            }
            await context.SaveChangesAsync();

            var controller = new CommentController(
                new CommentService(new CommentRepository(context), new TicketRepository(context), new Mock<INotificationService>().Object, new Mock<IChatPusher>().Object));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Role, "CLIENT"),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };

            var stopwatch = Stopwatch.StartNew();
            var result = await controller.GetCommentsForTicket(1);
            stopwatch.Stop();

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var comments = ok.Value.Should().BeAssignableTo<IEnumerable<CommentDto>>().Subject;
            comments.Should().HaveCount(100);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxLoadTimeMilliseconds,
                because: $"100 komentara mora se ucitati u < 2s — mjereno {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
