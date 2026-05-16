using Microsoft.AspNetCore.SignalR;
using TelecomSupportSystem.API.Hubs;
using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Workers
{
    public class ChatPusher : IChatPusher
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatPusher(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PushCommentAsync(int ticketId, CommentDto dto)
        {
            await _hubContext.Clients.Group($"ticket_{ticketId}").SendAsync("ReceiveComment", dto);
        }
    }
}