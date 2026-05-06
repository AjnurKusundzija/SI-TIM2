using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITicketRepository _ticketRepository;

        public CommentService(ICommentRepository commentRepository, ITicketRepository ticketRepository)
        {
            _commentRepository = commentRepository;
            _ticketRepository = ticketRepository;
        }

        // US-15: Ista logika pristupa kao i za tiket (CLIENT → vlastiti, AGENT/TECHNICIAN → dodijeljeni, ADMIN → svi)
        public async Task<IEnumerable<CommentDto>> GetCommentsForTicketAsync(int ticketId, int requestingUserId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Ticket {ticketId} not found.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" => true,
                "CLIENT"        => ticket.CreatorId == requestingUserId,
                "AGENT"         => ticket.Assignments.Any(a => a.UserId == requestingUserId),
                "TECHNICIAN"    => ticket.Assignments.Any(a => a.UserId == requestingUserId),
                _               => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Access to this ticket is not allowed.");

            var comments = await _commentRepository.GetByTicketIdAsync(ticketId);

            return comments.Select(c => new CommentDto
            {
                CommentId  = c.CommentId,
                Content    = c.Content,
                DateTime   = c.DateTime,
                AuthorId   = c.AuthorId,
                AuthorName = $"{c.Author.FirstName} {c.Author.LastName}",
                AuthorRole = c.Author.Role.ToString(),
            });
        }
    }
}