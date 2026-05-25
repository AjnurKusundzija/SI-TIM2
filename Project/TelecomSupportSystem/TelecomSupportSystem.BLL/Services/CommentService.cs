using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TelecomSupportSystem.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly INotificationService _notificationService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IChatPusher _chatPusher;

        public CommentService(
            ICommentRepository commentRepository,
            ITicketRepository ticketRepository,
            INotificationService notificationService,
            IAttachmentRepository attachmentRepository,
            IChatPusher chatPusher)
        {
            _commentRepository = commentRepository;
            _ticketRepository = ticketRepository;
            _notificationService = notificationService;
            _attachmentRepository = attachmentRepository;
            _chatPusher = chatPusher;
        }

        // PB-56: kompatibilni overload-i za postojeće testove koji ne koriste attachment funkcionalnost.
        public CommentService(
            ICommentRepository commentRepository,
            ITicketRepository ticketRepository,
            INotificationService notificationService,
            IChatPusher chatPusher)
            : this(commentRepository, ticketRepository, notificationService, new NullAttachmentRepository(), chatPusher)
        {
        }

        public CommentService(
            ICommentRepository commentRepository,
            ITicketRepository ticketRepository,
            INotificationService notificationService)
            : this(commentRepository, ticketRepository, notificationService, new NullAttachmentRepository(), new NullChatPusher())
        {
        }

        private sealed class NullChatPusher : IChatPusher
        {
            public System.Threading.Tasks.Task PushCommentAsync(int ticketId, CommentDto dto) => System.Threading.Tasks.Task.CompletedTask;
        }

        // US-15: Ista logika pristupa kao i za tiket (CLIENT → vlastiti, AGENT/TECHNICIAN → dodijeljeni, ADMIN → svi)
        public async Task<IEnumerable<CommentDto>> GetCommentsForTicketAsync(int ticketId, int requestingUserId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"        => ticket.CreatorId == requestingUserId,
                "TECHNICIAN"    => ticket.Assignments.Any(a => a.UserId == requestingUserId),
                _               => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Nemate pristup ovom tiketu.");

            var comments = await _commentRepository.GetByTicketIdAsync(ticketId);

            return comments.Select(c => new CommentDto
            {
                CommentId       = c.CommentId,
                Content         = c.Content,
                DateTime        = c.DateTime,
                AuthorId        = c.AuthorId,
                AuthorName      = c.IsSystemMessage ? string.Empty : $"{c.Author!.FirstName} {c.Author.LastName}",
                AuthorRole      = c.IsSystemMessage ? string.Empty : c.Author!.Role.ToString(),
                IsSystemMessage = c.IsSystemMessage,
                Attachments     = c.Attachments.Select(MapAttachment).ToList()
            });
        }

        private static AttachmentDto MapAttachment(Attachment a) => new()
        {
            AttachmentId = a.AttachmentId,
            FileName = a.FileName,
            ContentType = a.ContentType,
            Size = a.Size,
            UploadedAt = a.UploadedAt,
            DownloadUrl = $"/api/attachments/{a.AttachmentId}",
            UploadedByUserId = a.UserId,
            UploadedByName = a.User is null ? string.Empty : $"{a.User.FirstName} {a.User.LastName}".Trim()
        };

        public async Task<CommentDto> AddCommentAsync(int ticketId, int userId, string role, string content, IEnumerable<FileUploadDto>? attachments = null)
        {
            if (content.Length > 1000)
                throw new ArgumentException("Poruka ne može biti duža od 1000 znakova.");

            // PB-56: validacija prije bilo kakve write operacije
            if (attachments is not null && attachments.Any())
            {
                AttachmentStorage.Validate(attachments, AttachmentStorage.MaxAttachmentsPerComment);
            }

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"        => ticket.CreatorId == userId,
                "TECHNICIAN"    => ticket.Assignments.Any(a => a.UserId == userId),
                _               => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Nemate pristup ovom tiketu.");

            if (role == "CLIENT")
            {
                var existingComments = await _commentRepository.GetByTicketIdAsync(ticketId);
                int consecutiveClientComments = 0;

                foreach (var c in existingComments.OrderByDescending(x => x.DateTime))
                {
                    if (c.IsSystemMessage) continue;

                    if (c.Author!.Role.ToString() == "CLIENT")
                    {
                        consecutiveClientComments++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (consecutiveClientComments >= 3)
                {
                    throw new InvalidOperationException("Ne možete slati više od 3 uzastopne poruke. Sačekajte odgovor agenta.");
                }
            }

            // Basic sanitization
            var offensiveWords = new[]
            {
                "ass", "asshole", "bastard", "bitch", "bullshit",
                "dick", "cock", "pussy", "shit", "fuck",
                "motherfucker", "fucker", "damn", "prick",
                "douchebag", "jackass", "slut", "whore",
                "twat", "wank", "jerkoff", "handjob",
                "penis", "vagina", "cum", "cunt", "anal",
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

            if (attachments is not null && attachments.Any())
            {
                var (attachmentEntities, writtenPaths) =
                    AttachmentStorage.WriteFiles(attachments, ticketId, comment.CommentId, userId);

                try
                {
                    await _attachmentRepository.AddRangeAsync(attachmentEntities);
                }
                catch
                {
                    // US-80: ako DB save padne, počisti fajlove sa diska
                    AttachmentStorage.CleanupFiles(writtenPaths);
                    throw;
                }
            }

            // Fetch the created comment to get author details
            var createdComment = (await _commentRepository.GetByTicketIdAsync(ticketId)).Last();

            // TICKET_RESPONSE: klijent odgovori → notifikacija agentu/tehničaru; agent/tech odgovori → notifikacija klijentu
            if (role == "CLIENT")
            {
                var currentAssignee = ticket.Assignments
                    .OrderByDescending(a => a.AssignmentDate)
                    .FirstOrDefault();

                if (currentAssignee is not null)
                    await _notificationService.SendNotificationAsync(
                        currentAssignee.UserId,
                        "Novi odgovor klijenta",
                        $"Klijent je odgovorio na tiket \"{ticket.Title}\".",
                        NotificationType.TICKET_RESPONSE,
                        ticketId);
            }
            else if (role is "AGENT" or "TECHNICIAN")
            {
                await _notificationService.SendNotificationAsync(
                    ticket.CreatorId,
                    "Odgovor na vaš tiket",
                    $"Dobili ste odgovor na tiket \"{ticket.Title}\".",
                    NotificationType.TICKET_RESPONSE,
                    ticketId);
            }

            return new CommentDto
            {
                CommentId  = createdComment.CommentId,
                Content    = createdComment.Content,
                DateTime   = createdComment.DateTime,
                AuthorId   = createdComment.AuthorId,
                AuthorName = $"{createdComment.Author!.FirstName} {createdComment.Author.LastName}",
                AuthorRole = createdComment.Author.Role.ToString(),
                Attachments = createdComment.Attachments.Select(MapAttachment).ToList()
            };
        }

        public async Task AddSystemCommentAsync(int ticketId, string content)
        {
            var comment = new TelecomSupportSystem.DAL.Entities.Comment
            {
                TicketId        = ticketId,
                Content         = content,
                DateTime        = DateTime.UtcNow,
                IsSystemMessage = true,
                IsInternal      = false,
            };

            await _commentRepository.CreateAsync(comment);

            var dto = new CommentDto
            {
                CommentId       = comment.CommentId,
                Content         = comment.Content,
                DateTime        = comment.DateTime,
                IsSystemMessage = true,
            };

            await _chatPusher.PushCommentAsync(ticketId, dto);
        }
    }
}
