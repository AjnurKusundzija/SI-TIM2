using TelecomSupportSystem.BLL.DTOs.Comments;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        // US-15: Vraća komentare za tiket; pristup ovisi o roli (ista logika kao i za tiket)
        Task<IEnumerable<CommentDto>> GetCommentsForTicketAsync(int ticketId, int requestingUserId, string role);
        
        Task<CommentDto> AddCommentAsync(int ticketId, int userId, string role, string content);
        Task AddSystemCommentAsync(int ticketId, string content);
    }
}