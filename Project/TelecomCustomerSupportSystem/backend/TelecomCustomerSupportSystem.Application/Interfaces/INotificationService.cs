using TelecomCustomerSupportSystem.Application.DTOs.Notifications;

namespace TelecomCustomerSupportSystem.Application.Interfaces;

public interface INotificationService
{
    Task<bool> SendAsync(NotificationDto notificationDto, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<NotificationDto>> GetUnreadAsync(int korisnikId, CancellationToken cancellationToken = default);
}