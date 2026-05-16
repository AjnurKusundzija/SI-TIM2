using TelecomSupportSystem.BLL.DTOs.Notification;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<GetNotifcationDto>> GetNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task SendNotificationAsync(int userId, string title, string content, NotificationType type);
    }
}
