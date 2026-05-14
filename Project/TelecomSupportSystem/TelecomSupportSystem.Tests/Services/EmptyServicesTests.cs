using FluentAssertions;
using TelecomSupportSystem.BLL.Services;
using Xunit;

namespace TelecomSupportSystem.Tests.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSuccessfully()
        {
            // Arrange & Act
            var service = new NotificationService();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<TelecomSupportSystem.BLL.Services.Interfaces.INotificationService>();
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
            var service = new UserService();

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<TelecomSupportSystem.BLL.Services.Interfaces.IUserService>();
        }
    }
}
