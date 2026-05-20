using Microsoft.AspNetCore.SignalR;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.BLL.DTOs.Notification;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Workers
{
    public class NotificationPusher : INotificationPusher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationPusher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushAsync(int userId, GetNotifcationDto dto)
        {
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", dto);
        }
    }
}
