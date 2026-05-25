using TelecomSupportSystem.BLL.DTOs.Comments;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using TelecomSupportSystem.DAL.Entities;
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

        private static readonly string[] AllowedAttachmentExtensions =
        {
            ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".txt"
        };

        private const long MaxAttachmentSizeBytes = 5 * 1024 * 1024;
        private const int MaxCommentAttachments = 3;

        private static void ValidateAttachments(IEnumerable<FileUploadDto> attachments, int maxCount)
        {
            var attachmentList = attachments.ToList();
            if (attachmentList.Count > maxCount)
                throw new ArgumentException($"Moguće je poslati najviše {maxCount} priloga.");

            foreach (var attachment in attachmentList)
            {
                if (attachment.Data is null || attachment.Data.Length == 0)
                    throw new ArgumentException($"Prilog '{attachment.FileName}' je prazan.");
                if (attachment.Size > MaxAttachmentSizeBytes)
                    throw new ArgumentException($"Prilog '{attachment.FileName}' ne može biti veći od 5 MB.");

                var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();
                if (!AllowedAttachmentExtensions.Contains(extension))
                    throw new ArgumentException($"Prilog '{attachment.FileName}' nije podržan.");
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName) ?? "attachment";
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '_');
            }
            return name;
        }

        private static string GetStoredFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".bin";
            return $"{Guid.NewGuid():N}{extension}";
        }

        private static IEnumerable<Attachment> CreateAttachmentEntities(IEnumerable<FileUploadDto> uploads, int ticketId, int commentId)
        {
            var targetFolder = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var attachments = new List<Attachment>();
            foreach (var upload in uploads)
            {
                var sanitizedFile = SanitizeFileName(upload.FileName);
                var storedFileName = GetStoredFileName(sanitizedFile);
                var fullPath = Path.Combine(targetFolder, storedFileName);
                File.WriteAllBytes(fullPath, upload.Data);

                attachments.Add(new Attachment
                {
                    TicketId = ticketId,
                    CommentId = commentId,
                    FileName = sanitizedFile,
                    StoredFileName = storedFileName,
                    ContentType = upload.ContentType,
                    Size = upload.Size,
                    UploadedAt = DateTime.UtcNow
                });
            }

            return attachments;
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
                Attachments     = c.Attachments.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.Size,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/attachments/{a.AttachmentId}"
                }).ToList()
            });
        }

        public async Task<CommentDto> AddCommentAsync(int ticketId, int userId, string role, string content, IEnumerable<FileUploadDto>? attachments = null)
        {
            if (content.Length > 1000)
                throw new ArgumentException("Poruka ne može biti duža od 1000 znakova.");

            if (attachments is not null && attachments.Any())
            {
                ValidateAttachments(attachments, MaxCommentAttachments);
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

            if (attachments is not null && attachments.Any())
            {
                var attachmentEntities = CreateAttachmentEntities(attachments, ticketId, comment.CommentId);
                await _attachmentRepository.AddRangeAsync(attachmentEntities);
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
                Attachments = createdComment.Attachments.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.Size,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/attachments/{a.AttachmentId}"
                }).ToList()
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