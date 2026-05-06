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
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Integration
{
    // PB-27: Integracijsko testiranje komentara kroz sve slojeve
    public class CommentIntegrationTests
    {
        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static (CommentController controller, Mock<IHubContext<ChatHub>> hubMock)
            CreateController(ApplicationDbContext context, int userId, string role)
        {
            var ticketRepo = new TicketRepository(context);
            var commentRepo = new CommentRepository(context);
            var service = new CommentService(commentRepo, ticketRepo);
            var controller = new CommentController(service);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };

            var hubMock = new Mock<IHubContext<ChatHub>>();
            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);
            clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
            clientProxyMock.Setup(c => c.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            return (controller, hubMock);
        }

        private static User MakeUser(int id, string first, string last, Role role = Role.CLIENT) => new()
        {
            UserId = id,
            FirstName = first,
            LastName = last,
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = role,
        };

        private static Ticket MakeTicket(int id, int creatorId, User creator) => new()
        {
            TicketId = id,
            Title = "Test tiket",
            Description = "Opis problema.",
            CreatorId = creatorId,
            Creator = creator,
            Status = TicketStatus.OPEN,
            Priority = Priority.HIGH,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = DateTime.UtcNow,
            Assignments = new List<TicketUser>(),
        };

        // US-15: vlasnik tiketa moze dohvatiti komentare — lista se vraca ispravno
        [Fact]
        public async Task GetComments_OwnerOfTicket_ReturnsCommentList()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, "Merjem", "Omerovic");
            context.Users.Add(client);
            var ticket = MakeTicket(100, 1, client);
            context.Tickets.Add(ticket);
            context.Comments.Add(new Comment
            {
                CommentId = 1,
                TicketId = 100,
                AuthorId = 1,
                Author = client,
                Content = "Hvala na prijavi.",
                DateTime = DateTime.UtcNow,
                IsInternal = false,
            });
            await context.SaveChangesAsync();

            var (controller, _) = CreateController(context, userId: 1, role: "CLIENT");
            var result = await controller.GetCommentsForTicket(100);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var comments = ok.Value.Should().BeAssignableTo<IEnumerable<CommentDto>>().Subject.ToList();
            comments.Should().ContainSingle();
            comments[0].Content.Should().Be("Hvala na prijavi.");
            comments[0].AuthorName.Should().Be("Merjem Omerovic");
        }

        // PB-27 / US-20: slanje komentara persists u bazi i vraca DTO sa autorom
        [Fact]
        public async Task AddComment_ValidContent_PersistsAndReturnsDto()
        {
            using var context = CreateDbContext();
            var client = MakeUser(2, "Amir", "Hadzic");
            context.Users.Add(client);
            context.Tickets.Add(MakeTicket(200, 2, client));
            await context.SaveChangesAsync();

            var (controller, hubMock) = CreateController(context, userId: 2, role: "CLIENT");
            var result = await controller.AddComment(
                200,
                new CommentController.CreateCommentRequest { Content = "Problem i dalje postoji." },
                hubMock.Object);

            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            var dto = ok.Value.Should().BeAssignableTo<CommentDto>().Subject;
            dto.Content.Should().Be("Problem i dalje postoji.");
            dto.AuthorName.Should().Be("Amir Hadzic");
            context.Comments.Should().ContainSingle(c => c.TicketId == 200);
        }

        // PB-27 / US-20: poruka >1000 znakova vraca 400 BadRequest
        [Fact]
        public async Task AddComment_ContentTooLong_ReturnsBadRequest()
        {
            using var context = CreateDbContext();
            var client = MakeUser(3, "Jasmin", "Basic");
            context.Users.Add(client);
            context.Tickets.Add(MakeTicket(300, 3, client));
            await context.SaveChangesAsync();

            var (controller, hubMock) = CreateController(context, userId: 3, role: "CLIENT");
            var result = await controller.AddComment(
                300,
                new CommentController.CreateCommentRequest { Content = new string('a', 1001) },
                hubMock.Object);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // US-15: klijent ne moze pristupiti komentarima tudeg tiketa — vraca 403
        [Fact]
        public async Task GetComments_ClientAccessingOtherTicket_ReturnsForbid()
        {
            using var context = CreateDbContext();
            var owner = MakeUser(4, "Owner", "User");
            var intruder = MakeUser(5, "Intruder", "User");
            context.Users.AddRange(owner, intruder);
            context.Tickets.Add(MakeTicket(400, 4, owner));
            await context.SaveChangesAsync();

            var (controller, _) = CreateController(context, userId: 5, role: "CLIENT");
            var result = await controller.GetCommentsForTicket(400);

            result.Should().BeOfType<ForbidResult>();
        }
    }
}
