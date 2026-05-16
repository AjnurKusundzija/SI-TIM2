using TelecomSupportSystem.BLL.DTOs.Notification;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface INotificationPusher
    {
        Task PushAsync(int userId, GetNotifcationDto dto);
    }
}
