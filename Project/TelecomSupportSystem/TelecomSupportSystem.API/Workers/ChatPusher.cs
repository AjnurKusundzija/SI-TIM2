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

        // US-103: Interni komentari se broadcasta-ju isključivo u staff-only grupu.
        // Klijent se nikad ne pretplaćuje na "ticket_{id}_staff", tako da ovaj poziv
        // nikad ne stiže do klijentskog tracker-a.
        public async Task PushInternalCommentAsync(int ticketId, CommentDto dto)
        {
            await _hubContext.Clients.Group($"ticket_{ticketId}_staff").SendAsync("ReceiveInternalComment", dto);
        }
    }
}