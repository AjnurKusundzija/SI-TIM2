using TelecomSupportSystem.BLL.DTOs.Comments;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IChatPusher
    {
        Task PushCommentAsync(int ticketId, CommentDto dto);

        // US-103: Real-time isporuka internih komentara isključivo osoblju
        // (poseban SignalR grupni broadcast — klijent se nikad ne pretplaćuje na ovu grupu).
        Task PushInternalCommentAsync(int ticketId, CommentDto dto);
    }
}
