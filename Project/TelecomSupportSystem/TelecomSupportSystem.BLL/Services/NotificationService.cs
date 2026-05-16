using TelecomSupportSystem.BLL.DTOs.Notification;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationPusher _pusher;

        public NotificationService(INotificationRepository notificationRepository, INotificationPusher pusher)
        {
            _notificationRepository = notificationRepository;
            _pusher = pusher;
        }

        public async Task<IEnumerable<GetNotifcationDto>> GetNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return notifications.Select(MapToDto);
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException($"Notifikacija {notificationId} nije pronađena.");

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException("Nemate pristup ovoj notifikaciji.");

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task SendNotificationAsync(int userId, string title, string content, NotificationType type)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                NotificationType = type,
                SentDate = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationRepository.CreateAsync(notification);
            await _pusher.PushAsync(userId, MapToDto(notification));
        }

        private static GetNotifcationDto MapToDto(Notification n) => new()
        {
            NotificationId = n.NotificationId,
            Title = n.Title,
            Content = n.Content,
            NotificationType = n.NotificationType.ToString(),
            SentDate = n.SentDate,
            IsRead = n.IsRead
        };
    }
}
