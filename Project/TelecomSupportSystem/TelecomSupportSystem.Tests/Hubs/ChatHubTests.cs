using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using TelecomSupportSystem.API.Hubs;
using Xunit;

namespace TelecomSupportSystem.Tests.Hubs
{
    public class ChatHubTests
    {
        private readonly Mock<IGroupManager> _groupManagerMock;
        private readonly Mock<HubCallerContext> _contextMock;
        private readonly ChatHub _chatHub;

        public ChatHubTests()
        {
            _groupManagerMock = new Mock<IGroupManager>();
            _contextMock = new Mock<HubCallerContext>();
            _chatHub = new ChatHub
            {
                Groups = _groupManagerMock.Object,
                Context = _contextMock.Object
            };
        }

        [Fact]
        public async Task JoinTicketGroup_ShouldAddConnectionToGroup()
        {
            // Arrange
            var ticketId = "123";
            var connectionId = "connection_abc";
            var expectedGroupName = $"ticket_{ticketId}";

            _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

            _groupManagerMock
                .Setup(g => g.AddToGroupAsync(connectionId, expectedGroupName, default))
                .Returns(Task.CompletedTask);

            // Act
            await _chatHub.JoinTicketGroup(ticketId);

            // Assert
            _groupManagerMock.Verify(
                g => g.AddToGroupAsync(connectionId, expectedGroupName, default),
                Times.Once
            );
        }

        [Fact]
        public async Task LeaveTicketGroup_ShouldRemoveConnectionFromGroup()
        {
            // Arrange
            var ticketId = "456";
            var connectionId = "connection_xyz";
            var expectedGroupName = $"ticket_{ticketId}";

            _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

            _groupManagerMock
                .Setup(g => g.RemoveFromGroupAsync(connectionId, expectedGroupName, default))
                .Returns(Task.CompletedTask);

            // Act
            await _chatHub.LeaveTicketGroup(ticketId);

            // Assert
            _groupManagerMock.Verify(
                g => g.RemoveFromGroupAsync(connectionId, expectedGroupName, default),
                Times.Once
            );
        }

        [Fact]
        public async Task JoinTicketGroup_WithDifferentTickets_ShouldJoinDifferentGroups()
        {
            // Arrange
            var ticketId1 = "100";
            var ticketId2 = "200";
            var connectionId = "connection_test";

            _contextMock.Setup(c => c.ConnectionId).Returns(connectionId);

            _groupManagerMock
                .Setup(g => g.AddToGroupAsync(It.IsAny<string>(), It.IsAny<string>(), default))
                .Returns(Task.CompletedTask);

            // Act
            await _chatHub.JoinTicketGroup(ticketId1);
            await _chatHub.JoinTicketGroup(ticketId2);

            // Assert
            _groupManagerMock.Verify(
                g => g.AddToGroupAsync(connectionId, $"ticket_{ticketId1}", default),
                Times.Once
            );
            _groupManagerMock.Verify(
                g => g.AddToGroupAsync(connectionId, $"ticket_{ticketId2}", default),
                Times.Once
            );
        }
    }
}
