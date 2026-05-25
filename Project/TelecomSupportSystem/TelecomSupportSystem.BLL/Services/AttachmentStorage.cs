using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.BLL.Services
{
    // PB-56 / US-80: Centralizovan helper za validaciju, sanitaciju i upis attachment fajlova.
    // Koriste ga i TicketService i CommentService da bi pravila bila ista na oba mjesta.
    public static class AttachmentStorage
    {
        public const long MaxAttachmentSizeBytes = 5 * 1024 * 1024; // 5 MB
        public const int MaxAttachmentsPerTicket = 5;
        public const int MaxAttachmentsPerComment = 5;

        public static readonly IReadOnlyCollection<string> AllowedExtensions = new[]
        {
            ".png", ".jpg", ".jpeg", ".pdf", ".docx", ".txt"
        };

        // Eksplicitno zabranjeni izvršni / skriptni formati (US-80)
        public static readonly IReadOnlyCollection<string> ForbiddenExtensions = new[]
        {
            ".exe", ".bat", ".sh", ".cmd", ".com", ".msi", ".ps1", ".vbs", ".js"
        };

        private const string AttachmentsFolderName = "Attachments";

        public static string GetStorageFolder()
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), AttachmentsFolderName);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }

        public static void Validate(IEnumerable<FileUploadDto> attachments, int maxCount)
        {
            if (attachments is null)
                return;

            var list = attachments.ToList();

            if (list.Count > maxCount)
                throw new ArgumentException($"Moguće je poslati najviše {maxCount} priloga.");

            foreach (var attachment in list)
            {
                if (attachment.Data is null || attachment.Data.Length == 0)
                    throw new ArgumentException($"Prilog '{attachment.FileName}' je prazan.");

                if (attachment.Size > MaxAttachmentSizeBytes)
                    throw new ArgumentException($"Prilog '{attachment.FileName}' ne može biti veći od 5 MB.");

                var extension = Path.GetExtension(attachment.FileName).ToLowerInvariant();

                if (ForbiddenExtensions.Contains(extension))
                    throw new ArgumentException($"Upload izvršnih fajlova ({extension}) je zabranjen.");

                if (!AllowedExtensions.Contains(extension))
                    throw new ArgumentException($"Format '{extension}' nije podržan. Dozvoljeni: PNG, JPG, JPEG, PDF, DOCX, TXT.");
            }
        }

        // PB-56: sanitizacija imena fajla — zaštita od path traversal-a + zamjena opasnih karaktera.
        // Path.GetFileName na Linuxu ne tretira '\' kao separator, pa cross-platform najprije ručno
        // strip-amo putanju (i '/' i '\'), tek onda primijenimo OS-specifičnu validaciju karaktera.
        public static string SanitizeFileName(string fileName)
        {
            var input = fileName ?? string.Empty;

            // Normaliziraj sve slash-eve i uzmi posljednji segment — radi i na Linuxu i na Windowsu.
            var lastSlash = input.LastIndexOfAny(new[] { '/', '\\' });
            var name = lastSlash >= 0 ? input.Substring(lastSlash + 1) : input;

            // Drive prefix (npr. "C:" ako neko proslijedi "C:zlocesto.txt" bez slash-a) takođe odbacujemo.
            if (name.Length >= 2 && name[1] == ':')
                name = name.Substring(2);

            if (string.IsNullOrWhiteSpace(name))
                name = "prilog";

            foreach (var invalidChar in Path.GetInvalidFileNameChars())
                name = name.Replace(invalidChar, '_');

            // Dodatni opasni karakteri koje OS dozvoljava ali ne želimo u URL-u / nazivu
            foreach (var c in new[] { ' ', '\t', '#', '%', '&', '{', '}', '$', '!', '\'', '"', ':', '@', '+', '`', '|', '=' })
                name = name.Replace(c, '_');

            // Spriječi izlazak iz foldera kroz tačke
            while (name.StartsWith("."))
                name = name.TrimStart('.');

            if (string.IsNullOrWhiteSpace(name))
                name = $"prilog_{Guid.NewGuid():N}";

            return name;
        }

        public static string GenerateStoredFileName(string sanitizedFileName)
        {
            var extension = Path.GetExtension(sanitizedFileName);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".bin";
            return $"{Guid.NewGuid():N}{extension}";
        }

        // Upisuje fajlove na disk i vraća listu Attachment entiteta koji još nisu sačuvani u DB.
        // Vraća i listu apsolutnih putanja kako bi se mogle obrisati ako DB save padne (US-80 sigurnost).
        public static (List<Attachment> Entities, List<string> WrittenPaths) WriteFiles(
            IEnumerable<FileUploadDto> uploads,
            int? ticketId,
            int? commentId,
            int? uploaderUserId)
        {
            var folder = GetStorageFolder();
            var entities = new List<Attachment>();
            var writtenPaths = new List<string>();

            foreach (var upload in uploads)
            {
                var sanitizedFile = SanitizeFileName(upload.FileName);
                var storedFileName = GenerateStoredFileName(sanitizedFile);
                var fullPath = Path.Combine(folder, storedFileName);

                File.WriteAllBytes(fullPath, upload.Data);
                writtenPaths.Add(fullPath);

                entities.Add(new Attachment
                {
                    TicketId = ticketId,
                    CommentId = commentId,
                    UserId = uploaderUserId,
                    FileName = sanitizedFile,
                    StoredFileName = storedFileName,
                    FilePath = fullPath,
                    ContentType = string.IsNullOrWhiteSpace(upload.ContentType)
                        ? "application/octet-stream"
                        : upload.ContentType,
                    Size = upload.Size > 0 ? upload.Size : upload.Data.LongLength,
                    UploadedAt = DateTime.UtcNow
                });
            }

            return (entities, writtenPaths);
        }

        // PB-56: cleanup orphan fajlova ako DB save padne nakon upisa na disk.
        public static void CleanupFiles(IEnumerable<string> paths)
        {
            if (paths is null) return;
            foreach (var path in paths)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch
                {
                    // best-effort cleanup
                }
            }
        }
    }
}
