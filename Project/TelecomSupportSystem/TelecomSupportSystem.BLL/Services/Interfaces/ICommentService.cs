using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.DTOs.Attachments;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        // US-15 / US-103: Vraća komentare za tiket; pristup ovisi o roli (ista logika kao i za tiket).
        // Za CLIENT-a se interni komentari uvijek izostavljaju; osoblje dobija kompletan hronološki tok.
        Task<IEnumerable<CommentDto>> GetCommentsForTicketAsync(int ticketId, int requestingUserId, string role);

        Task<CommentDto> AddCommentAsync(int ticketId, int userId, string role, string content, IEnumerable<FileUploadDto>? attachments = null);
        Task AddSystemCommentAsync(int ticketId, string content);

        // US-102: Dodaje interni komentar (vidljiv samo osoblju). Dozvoljeno za AGENT, TECHNICIAN, ADMINISTRATOR.
        // Baca UnauthorizedAccessException za neovlaštenu rolu i InvalidOperationException ako je tiket zatvoren.
        Task<CommentDto> AddInternalCommentAsync(int ticketId, int userId, string role, string content);
    }
}