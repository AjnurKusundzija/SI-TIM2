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
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"        => ticket.CreatorId == requestingUserId,
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

        public async Task<CommentDto> AddCommentAsync(int ticketId, int userId, string role, string content)
        {
            if (content.Length > 1000)
                throw new ArgumentException("Message exceeds maximum length of 1000 characters.");

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Ticket {ticketId} not found.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"        => ticket.CreatorId == userId,
                "TECHNICIAN"    => ticket.Assignments.Any(a => a.UserId == userId),
                _               => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Access to this ticket is not allowed.");

            // Basic sanitization
            var offensiveWords = new[]
            {
                // English common profanity
                "ass", "asshole", "bastard", "bitch", "bullshit",
                "dick", "cock", "pussy", "shit", "fuck",
                "motherfucker", "fucker", "damn", "prick",
                "douchebag", "jackass", "slut", "whore",
                "twat", "wank", "jerkoff", "handjob",

                // Sexual explicit terms
                "penis", "vagina", "cum", "cunt", "anal",

                // Bosnian / regional profanity
                "jebem", "jebo", "jebi", "jebiga",
                "kurac", "pička", "picka", "pickica",
                "sranje", "serem", "seronja",
                "govno", "kreten", "budala",
                "moron", "idiot"
            };
            foreach (var word in offensiveWords)
            {
                content = System.Text.RegularExpressions.Regex.Replace(content, $@"\b{word}\b", "***", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            var comment = new TelecomSupportSystem.DAL.Entities.Comment
            {
                TicketId = ticketId,
                AuthorId = userId,
                Content = content,
                DateTime = DateTime.UtcNow,
                IsInternal = false
            };

            await _commentRepository.CreateAsync(comment);

            // Fetch the created comment to get author details
            var createdComment = (await _commentRepository.GetByTicketIdAsync(ticketId)).Last();

            return new CommentDto
            {
                CommentId = createdComment.CommentId,
                Content = createdComment.Content,
                DateTime = createdComment.DateTime,
                AuthorId = createdComment.AuthorId,
                AuthorName = $"{createdComment.Author.FirstName} {createdComment.Author.LastName}",
                AuthorRole = createdComment.Author.Role.ToString()
            };
        }
    }
}