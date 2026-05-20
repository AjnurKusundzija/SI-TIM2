using TelecomSupportSystem.BLL.DTOs.Comments;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IChatPusher
    {
        Task PushCommentAsync(int ticketId, CommentDto dto);
    }
}
