using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TelecomSupportSystem.API.Controllers;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories;
using Xunit;

namespace TelecomSupportSystem.Tests.Attachments
{
    // PB-56 / US-80, US-81: Integracijski testovi za upload, validaciju i preuzimanje priloga.
    public class AttachmentTests : IDisposable
    {
        private readonly string _tempCwd;
        private readonly string _originalCwd;

        public AttachmentTests()
        {
            // AttachmentStorage upisuje u <CWD>/Attachments — koristimo izolovan privremeni folder po test klasi
            // da ne zagađujemo radni direktorij i da možemo provjeriti orphan cleanup logiku.
            _originalCwd = Directory.GetCurrentDirectory();
            _tempCwd = Path.Combine(Path.GetTempPath(), "tss_attachments_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempCwd);
            Directory.SetCurrentDirectory(_tempCwd);
        }

        public void Dispose()
        {
            Directory.SetCurrentDirectory(_originalCwd);
            try { if (Directory.Exists(_tempCwd)) Directory.Delete(_tempCwd, recursive: true); } catch { }
        }

        private static ApplicationDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        private static User MakeUser(int id, Role role = Role.CLIENT, string first = "Test", string last = "User") => new()
        {
            UserId = id,
            FirstName = first,
            LastName = last,
            Email = $"u{id}@test.ba",
            Username = $"u{id}",
            PasswordHash = "hash",
            AccountStatus = AccountStatus.ACTIVE,
            Role = role
        };

        private static FileUploadDto MakeUpload(string fileName, int sizeBytes = 100, string? contentType = null)
        {
            var data = new byte[sizeBytes];
            new Random(42).NextBytes(data);
            return new FileUploadDto
            {
                FileName = fileName,
                ContentType = contentType ?? "application/octet-stream",
                Data = data,
                Size = data.LongLength
            };
        }

        private static TicketController CreateTicketController(ApplicationDbContext context, int userId, string role = "CLIENT")
        {
            var ticketRepo = new TicketRepository(context);
            var teamRepo = new TeamRepository(context);
            var userRepo = new UserRepository(context);
            var attachmentRepo = new AttachmentRepository(context);
            var notification = new Mock<INotificationService>().Object;
            var commentService = new Mock<ICommentService>().Object;
            var ticketService = new TicketService(ticketRepo, teamRepo, userRepo, notification, attachmentRepo, commentService);

            var controller = new TicketController(ticketService);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };
            return controller;
        }

        private static CommentService CreateCommentService(ApplicationDbContext context)
        {
            return new CommentService(
                new CommentRepository(context),
                new TicketRepository(context),
                new Mock<INotificationService>().Object,
                new AttachmentRepository(context),
                new Mock<IChatPusher>().Object);
        }

        private static AttachmentsController CreateAttachmentsController(ApplicationDbContext context, int userId, string role)
        {
            var controller = new AttachmentsController(new AttachmentRepository(context));
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Role, role),
            };
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) }
            };
            return controller;
        }

        // ─── US-80: VALIDACIJA ─────────────────────────────────────────────────

        [Fact]
        public void Validate_AllowsKnownGoodFormats()
        {
            var ok = new List<FileUploadDto>
            {
                MakeUpload("slika.png"),
                MakeUpload("dokument.pdf"),
                MakeUpload("napomena.txt"),
                MakeUpload("ugovor.docx"),
                MakeUpload("photo.jpeg"),
            };
            var act = () => AttachmentStorage.Validate(ok, AttachmentStorage.MaxAttachmentsPerTicket);
            act.Should().NotThrow();
        }

        [Fact]
        public void Validate_RejectsXlsx_NoLongerAllowed()
        {
            var act = () => AttachmentStorage.Validate(new[] { MakeUpload("tabela.xlsx") }, 5);
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(".exe")]
        [InlineData(".bat")]
        [InlineData(".sh")]
        [InlineData(".cmd")]
        [InlineData(".com")]
        [InlineData(".ps1")]
        [InlineData(".msi")]
        public void Validate_RejectsExecutableExtensions(string extension)
        {
            var act = () => AttachmentStorage.Validate(new[] { MakeUpload($"virus{extension}") }, 5);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*zabranjen*");
        }

        [Fact]
        public void Validate_RejectsMoreThan5Files()
        {
            var files = Enumerable.Range(0, 6).Select(i => MakeUpload($"slika{i}.png")).ToList();
            var act = () => AttachmentStorage.Validate(files, AttachmentStorage.MaxAttachmentsPerTicket);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*najviše 5*");
        }

        [Fact]
        public void Validate_RejectsFileOver5Mb()
        {
            // 6 MB
            var huge = MakeUpload("velika.png", 6 * 1024 * 1024);
            var act = () => AttachmentStorage.Validate(new[] { huge }, 5);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*5 MB*");
        }

        [Fact]
        public void Validate_RejectsUnsupportedExtension()
        {
            var act = () => AttachmentStorage.Validate(new[] { MakeUpload("arhiva.zip") }, 5);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*nije podržan*");
        }

        // ─── SANITIZACIJA NAZIVA FAJLA ──────────────────────────────────────────

        [Theory]
        [InlineData("../../etc/passwd.png", "passwd.png")]
        [InlineData("C:\\windows\\zlocesto.txt", "zlocesto.txt")]
        [InlineData("normalni naziv.pdf", "normalni_naziv.pdf")]
        public void SanitizeFileName_StripsPath_AndReplacesSpaces(string input, string expected)
        {
            AttachmentStorage.SanitizeFileName(input).Should().Be(expected);
        }

        [Fact]
        public void SanitizeFileName_ReplacesDangerousCharacters()
        {
            var result = AttachmentStorage.SanitizeFileName("a&b#c$d%e.png");
            result.Should().NotContain("&").And.NotContain("#").And.NotContain("$").And.NotContain("%");
            result.Should().EndWith(".png");
        }

        // ─── US-80: UPLOAD KROZ SERVIS ─────────────────────────────────────────

        [Fact]
        public async Task CreateTicket_WithValidAttachments_PersistsAttachmentsWithUploader()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1));
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");

            var result = await controller.CreateTicketWithAttachments(new TicketController.CreateTicketWithAttachmentsDto
            {
                Subject = "Internet ne radi",
                Description = "Detaljan opis",
                Priority = Priority.HIGH,
                Type = ProblemCategory.INTERNET,
                Attachments = new FormFileCollectionStub
                {
                    new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "Attachments", "slika.png")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/png"
                    }
                }
            });

            result.Should().BeOfType<CreatedAtActionResult>();
            var attachments = context.Attachments.ToList();
            attachments.Should().HaveCount(1);
            attachments[0].UserId.Should().Be(1);
            attachments[0].FileName.Should().Be("slika.png");
            attachments[0].TicketId.Should().NotBeNull();
        }

        [Fact]
        public async Task AddComment_WithValidAttachments_PersistsAttachmentsWithUploader()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1);
            context.Users.Add(client);
            context.Tickets.Add(new Ticket
            {
                TicketId = 10,
                Title = "T",
                Description = "D",
                CreatorId = 1,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var service = CreateCommentService(context);
            var result = await service.AddCommentAsync(10, 1, "CLIENT", "uz prilog",
                new[] { MakeUpload("note.txt", 50, "text/plain") });

            result.Attachments.Should().HaveCount(1);
            var saved = context.Attachments.Single();
            saved.UserId.Should().Be(1);
            saved.CommentId.Should().NotBeNull();
            saved.TicketId.Should().Be(10);
        }

        [Fact]
        public async Task AddComment_RejectsMoreThan5Attachments()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1);
            context.Users.Add(client);
            context.Tickets.Add(new Ticket
            {
                TicketId = 11,
                Title = "T",
                Description = "D",
                CreatorId = 1,
                Creator = client,
                Status = TicketStatus.OPEN,
                Priority = Priority.LOW,
                ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var service = CreateCommentService(context);
            var manyFiles = Enumerable.Range(0, 6).Select(i => MakeUpload($"x{i}.png")).ToList();
            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddCommentAsync(11, 1, "CLIENT", "spam", manyFiles));
        }

        // ─── US-81: DOWNLOAD ACCESS CONTROL ────────────────────────────────────

        [Fact]
        public async Task Download_AllowsClient_WhenOwnerOfTicket()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1);
            context.Users.Add(client);
            var ticket = new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1, Creator = client,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            };
            context.Tickets.Add(ticket);

            // upiši fizički fajl u izolovani CWD/Attachments
            var folder = AttachmentStorage.GetStorageFolder();
            var stored = Guid.NewGuid().ToString("N") + ".txt";
            var path = Path.Combine(folder, stored);
            File.WriteAllText(path, "hello");

            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "ok.txt", StoredFileName = stored, FilePath = path,
                Size = 5, ContentType = "text/plain", UploadedAt = DateTime.UtcNow,
                UserId = 1, TicketId = 1
            });
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 1, role: "CLIENT");
            var result = await controller.Download(1);
            result.Should().BeOfType<PhysicalFileResult>();
        }

        [Fact]
        public async Task Download_ForbidsClient_WhenNotOwner()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1));
            var owner = MakeUser(2);
            context.Users.Add(owner);
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 2, Creator = owner,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            });
            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "tajno.txt", StoredFileName = "x.txt", FilePath = "x.txt",
                Size = 1, ContentType = "text/plain", UploadedAt = DateTime.UtcNow,
                UserId = 2, TicketId = 1
            });
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 1, role: "CLIENT");
            var result = await controller.Download(1);
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Download_AllowsAgent_WhenAssignedToTicket()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1);
            var agent = MakeUser(5, Role.AGENT);
            context.Users.AddRange(client, agent);

            var ticket = new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1, Creator = client,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>
                {
                    new() { UserId = 5, AssignmentDate = DateTime.UtcNow, AssignmentType = AssignmentType.AUTOMATIC }
                }
            };
            context.Tickets.Add(ticket);

            var folder = AttachmentStorage.GetStorageFolder();
            var stored = Guid.NewGuid().ToString("N") + ".txt";
            var path = Path.Combine(folder, stored);
            File.WriteAllText(path, "x");

            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "f.txt", StoredFileName = stored, FilePath = path,
                Size = 1, ContentType = "text/plain", UploadedAt = DateTime.UtcNow,
                UserId = 1, TicketId = 1
            });
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 5, role: "AGENT");
            var result = await controller.Download(1);
            result.Should().BeOfType<PhysicalFileResult>();
        }

        [Fact]
        public async Task Download_ForbidsAgent_WhenNotAssigned()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1);
            var agent = MakeUser(5, Role.AGENT);
            context.Users.AddRange(client, agent);

            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1, Creator = client,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>()
            });

            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "f.txt", StoredFileName = "x.txt", FilePath = "x.txt",
                Size = 1, ContentType = "text/plain", UploadedAt = DateTime.UtcNow,
                UserId = 1, TicketId = 1
            });
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 5, role: "AGENT");
            var result = await controller.Download(1);
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public async Task Download_ReturnsNotFound_WhenAttachmentMissing()
        {
            using var context = CreateDbContext();
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.Download(999);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Download_ReturnsNotFound_WhenPhysicalFileMissing()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1, Role.ADMINISTRATOR));
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow
            });
            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "missing.txt", StoredFileName = "missing-stored.txt",
                FilePath = Path.Combine(AttachmentStorage.GetStorageFolder(), "missing-stored.txt"),
                Size = 1, ContentType = "text/plain", UploadedAt = DateTime.UtcNow,
                UserId = 1, TicketId = 1
            });
            await context.SaveChangesAsync();

            var controller = CreateAttachmentsController(context, userId: 1, role: "ADMINISTRATOR");
            var result = await controller.Download(1);
            result.Should().BeOfType<NotFoundResult>();
        }

        // ─── US-81: ATTACHMENTDTO METAPODACI ────────────────────────────────────

        [Fact]
        public async Task GetTicketDetail_AttachmentDto_ContainsFilenameSizeUploadedAtAndUploadedBy()
        {
            using var context = CreateDbContext();
            var client = MakeUser(1, first: "Amina", last: "Begić");
            context.Users.Add(client);
            context.Tickets.Add(new Ticket
            {
                TicketId = 1, Title = "T", Description = "D", CreatorId = 1, Creator = client,
                Status = TicketStatus.OPEN, Priority = Priority.LOW, ProblemCategory = ProblemCategory.INTERNET,
                CreatedDate = DateTime.UtcNow,
                Assignments = new List<TicketUser>()
            });
            context.Attachments.Add(new Attachment
            {
                AttachmentId = 1, FileName = "moje.pdf", StoredFileName = "stored.pdf",
                Size = 1234, ContentType = "application/pdf",
                UploadedAt = DateTime.UtcNow.AddMinutes(-3),
                UserId = 1, TicketId = 1
            });
            await context.SaveChangesAsync();

            var ticketRepo = new TicketRepository(context);
            var service = new TicketService(
                ticketRepo,
                new TeamRepository(context),
                new UserRepository(context),
                new Mock<INotificationService>().Object,
                new AttachmentRepository(context),
                new Mock<ICommentService>().Object);

            var detail = await service.GetTicketByIdAsync(1, 1, "CLIENT");

            detail.Attachments.Should().HaveCount(1);
            var a = detail.Attachments.Single();
            a.FileName.Should().Be("moje.pdf");
            a.Size.Should().Be(1234);
            a.UploadedAt.Should().BeBefore(DateTime.UtcNow);
            a.UploadedByUserId.Should().Be(1);
            a.UploadedByName.Should().Be("Amina Begić");
            a.DownloadUrl.Should().Be("/api/attachments/1");
        }

        // PB-56: ako Validate baci grešku PRIJE upisa, nema fajlova na disku — provjeravamo da nema orphan-a.
        [Fact]
        public async Task CreateTicket_WhenValidationFails_NoOrphanFilesAreLeft()
        {
            using var context = CreateDbContext();
            context.Users.Add(MakeUser(1));
            await context.SaveChangesAsync();

            var controller = CreateTicketController(context, userId: 1, role: "CLIENT");

            var folder = AttachmentStorage.GetStorageFolder();
            var before = Directory.GetFiles(folder).Length;

            var act = async () => await controller.CreateTicketWithAttachments(new TicketController.CreateTicketWithAttachmentsDto
            {
                Subject = "T", Description = "D", Priority = Priority.LOW, Type = ProblemCategory.INTERNET,
                Attachments = new FormFileCollectionStub
                {
                    new FormFile(new MemoryStream(new byte[] { 1 }), 0, 1, "Attachments", "virus.exe")
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/octet-stream"
                    }
                }
            });

            await act.Should().ThrowAsync<ArgumentException>();

            Directory.GetFiles(folder).Length.Should().Be(before);
        }
    }

    // Pomoćni helper za testove: omogućuje konstrukciju IFormFileCollection sa initializerima.
    internal sealed class FormFileCollectionStub : List<IFormFile>, IFormFileCollection
    {
        public IFormFile? this[string name] => this.FirstOrDefault(f => f.Name == name);
        public IFormFile? GetFile(string name) => this[name];
        public IReadOnlyList<IFormFile> GetFiles(string name) => this.Where(f => f.Name == name).ToList();
    }
}
