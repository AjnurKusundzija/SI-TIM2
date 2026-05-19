using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;
using NotificationType = TelecomSupportSystem.DAL.Entities.Enums.NotificationType;

namespace TelecomSupportSystem.Tests.TicketT
{
    // PB-36 / US-60: Tehničar mijenja status tiketa koji mu je dodijeljen
    public class TicketStatusUpdateServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
        private readonly Mock<ITeamRepository> _teamRepoMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<ICommentService> _commentServiceMock = new();
        private readonly TicketService _ticketService;

        private const int TechnicianId = 7;
        private const int CreatorId = 42;
        private const int TicketId = 10;

        public TicketStatusUpdateServiceTests()
        {
            _ticketService = new TicketService(
                _ticketRepositoryMock.Object,
                _teamRepoMock.Object,
                _userRepoMock.Object,
                _notificationServiceMock.Object,
                _commentServiceMock.Object);
        }

        private static Ticket MakeAssignedTicket(
            int assigneeId,
            TicketStatus status = TicketStatus.OPEN) => new()
        {
            TicketId = TicketId,
            Title = "Test tiket",
            Description = "Opis",
            CreatorId = CreatorId,
            Status = status,
            Priority = Priority.MEDIUM,
            ProblemCategory = ProblemCategory.INTERNET,
            CreatedDate = DateTime.UtcNow,
            Assignments = new List<TicketUser>
            {
                new()
                {
                    TicketId = TicketId,
                    UserId = assigneeId,
                    AssignmentDate = DateTime.UtcNow,
                    AssignmentType = AssignmentType.AUTOMATIC,
                    User = new User { UserId = assigneeId, FirstName = "T", LastName = "T" }
                }
            }
        };

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldUpdateStatusAndNotifyCreator_WhenAssignedTechnicianRequestsClosure()
        {
            var ticket = MakeAssignedTicket(TechnicianId, TicketStatus.OPEN);
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSURE_REQUESTED, TechnicianId, "TECHNICIAN");

            ticket.Status.Should().Be(TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestedById.Should().Be(TechnicianId);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.PENDING);
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(ticket), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                CreatorId,
                It.IsAny<string>(),
                It.IsAny<string>(),
                NotificationType.STATUS_CHANGED,
                TicketId), Times.Once);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenTicketNotAssignedToTechnician()
        {
            var ticket = MakeAssignedTicket(assigneeId: 99, TicketStatus.OPEN); // assigned to someone else
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            var act = () => _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSURE_REQUESTED, TechnicianId, "TECHNICIAN");

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>(), It.IsAny<int?>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTicketIsClosed()
        {
            var ticket = MakeAssignedTicket(TechnicianId, TicketStatus.CLOSED);
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            var act = () => _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSURE_REQUESTED, TechnicianId, "TECHNICIAN");

            await act.Should().ThrowAsync<InvalidOperationException>();
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>(), It.IsAny<int?>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowUnauthorized_WhenRoleIsNotTechnician()
        {
            var act = () => _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSURE_REQUESTED, TechnicianId, "CLIENT");

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
            _ticketRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowInvalidOperation_WhenTargetStatusNotAllowed()
        {
            // CLOSED is not in the technician's allowed list — closure must go through client confirmation
            var ticket = MakeAssignedTicket(TechnicianId, TicketStatus.OPEN);
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            var act = () => _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSED, TechnicianId, "TECHNICIAN");

            await act.Should().ThrowAsync<InvalidOperationException>();
            _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldThrowKeyNotFound_WhenTicketDoesNotExist()
        {
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync((Ticket?)null);

            var act = () => _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.CLOSURE_REQUESTED, TechnicianId, "TECHNICIAN");

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldBeNoOp_WhenNewStatusEqualsCurrent()
        {
            var ticket = MakeAssignedTicket(TechnicianId, TicketStatus.OPEN);
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.OPEN, TechnicianId, "TECHNICIAN");

            _ticketRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<NotificationType>(), It.IsAny<int?>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTicketStatusAsync_ShouldMarkClosureAsRejected_WhenMovingFromClosureRequestedBackToOpen()
        {
            var ticket = MakeAssignedTicket(TechnicianId, TicketStatus.CLOSURE_REQUESTED);
            ticket.ClosureRequestStatus = ClosureRequestStatus.PENDING;
            _ticketRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(TicketId)).ReturnsAsync(ticket);

            await _ticketService.UpdateTicketStatusAsync(TicketId, TicketStatus.OPEN, TechnicianId, "TECHNICIAN");

            ticket.Status.Should().Be(TicketStatus.OPEN);
            ticket.ClosureRequestStatus.Should().Be(ClosureRequestStatus.REJECTED);
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(
                CreatorId, It.IsAny<string>(), It.IsAny<string>(),
                NotificationType.STATUS_CHANGED, TicketId), Times.Once);
        }
    }

    // PB-36 / US-60: Kontroler endpoint za promjenu statusa
    public class TicketStatusUpdateControllerTests
    {
        private readonly Mock<ITicketService> _ticketServiceMock = new();
        private readonly TicketController _controller;

        public TicketStatusUpdateControllerTests()
        {
            _controller = new TicketController(_ticketServiceMock.Object);
        }

        private void SetUser(int userId, string role)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnOk_WhenValid()
        {
            SetUser(7, "TECHNICIAN");
            var dto = new UpdateTicketStatusDto { Status = TicketStatus.CLOSURE_REQUESTED };

            _ticketServiceMock.Setup(s => s.UpdateTicketStatusAsync(10, dto.Status, 7, "TECHNICIAN"))
                .Returns(Task.CompletedTask);

            var result = await _controller.UpdateStatus(10, dto);

            result.Should().BeOfType<OkObjectResult>();
            _ticketServiceMock.Verify(s => s.UpdateTicketStatusAsync(10, dto.Status, 7, "TECHNICIAN"), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnForbid_WhenServiceThrowsUnauthorized()
        {
            SetUser(7, "TECHNICIAN");
            var dto = new UpdateTicketStatusDto { Status = TicketStatus.CLOSURE_REQUESTED };

            _ticketServiceMock.Setup(s => s.UpdateTicketStatusAsync(10, dto.Status, 7, "TECHNICIAN"))
                .ThrowsAsync(new UnauthorizedAccessException());

            var result = await _controller.UpdateStatus(10, dto);

            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnBadRequest_WhenServiceThrowsInvalidOperation()
        {
            SetUser(7, "TECHNICIAN");
            var dto = new UpdateTicketStatusDto { Status = TicketStatus.CLOSURE_REQUESTED };

            _ticketServiceMock.Setup(s => s.UpdateTicketStatusAsync(10, dto.Status, 7, "TECHNICIAN"))
                .ThrowsAsync(new InvalidOperationException("Tiket je već zatvoren."));

            var result = await _controller.UpdateStatus(10, dto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnNotFound_WhenTicketDoesNotExist()
        {
            SetUser(7, "TECHNICIAN");
            var dto = new UpdateTicketStatusDto { Status = TicketStatus.CLOSURE_REQUESTED };

            _ticketServiceMock.Setup(s => s.UpdateTicketStatusAsync(999, dto.Status, 7, "TECHNICIAN"))
                .ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.UpdateStatus(999, dto);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task UpdateStatus_ShouldReturnUnauthorized_WhenNoUserClaims()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };
            var dto = new UpdateTicketStatusDto { Status = TicketStatus.CLOSURE_REQUESTED };

            var result = await _controller.UpdateStatus(10, dto);

            result.Should().BeOfType<UnauthorizedResult>();
        }
    }
}
