using FluentAssertions;
using Moq;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var service = new NotificationService(
                new Mock<INotificationRepository>().Object,
                new Mock<INotificationPusher>().Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<INotificationService>();
        }
    }

    public class ReportServiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var service = new ReportService();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<TelecomSupportSystem.BLL.Services.Interfaces.IReportService>();
        }
    }

    public class UserServiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var service = new UserService(
                new Mock<ITicketRepository>().Object,
                new Mock<IUserRepository>().Object,
                new Mock<IPackageService>().Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<TelecomSupportSystem.BLL.Services.Interfaces.IUserService>();
        }
    }
}
