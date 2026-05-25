using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TelecomSupportSystem.BLL.DTOs.Attachments;
using NotificationType = TelecomSupportSystem.DAL.Entities.Enums.NotificationType;

namespace TelecomSupportSystem.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ICommentService _commentService;

        public TicketService(
            ITicketRepository ticketRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IAttachmentRepository attachmentRepository,
            ICommentService commentService)
        {
            _ticketRepository    = ticketRepository;
            _teamRepository      = teamRepository;
            _userRepository      = userRepository;
            _notificationService = notificationService;
            _attachmentRepository = attachmentRepository;
            _commentService      = commentService;
        }

        // PB-56: kompatibilni overload za postojeće testove koji ne testiraju attachment funkcionalnost.
        // Tester koji ne treba pratiti upload priloga ne mora mockovati IAttachmentRepository.
        public TicketService(
            ITicketRepository ticketRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            ICommentService commentService)
            : this(ticketRepository, teamRepository, userRepository, notificationService,
                   new TelecomSupportSystem.DAL.Repositories.NullAttachmentRepository(), commentService)
        {
        }

        // US-11: Dohvata tikete iz repozitorija i mapira ih u DTO.
        // Enum se pretvara u string (.ToString()) da frontend ne mora
        // raditi numeričko mapiranje (npr. 0 → "OPEN").
        public async Task<IEnumerable<MyTicketDto>> GetMyTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetByCreatorIdAsync(userId);

            return tickets.Select(t => new MyTicketDto
            {
                TicketId        = t.TicketId,
                Title           = t.Title,
                Description     = t.Description,
                Status          = t.Status.ToString(),
                Priority        = t.Priority.ToString(),
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate
            });
        }

        private static bool IsActiveAssignee(Ticket ticket, int userId)
        {
            var orderedAssignments = ticket.Assignments
                .OrderBy(a => a.AssignmentDate)
                .ThenBy(a => a.AssignmentId)
                .ToList();

            var latestAssignment = orderedAssignments.LastOrDefault();
            if (latestAssignment is null)
                return false;

            if (latestAssignment.UserId == userId)
                return true;

            if (latestAssignment.AssignmentType != AssignmentType.FORWARDED_TO_TECHNICIAN)
                return false;

            var previousAssignment = orderedAssignments.Count > 1 ? orderedAssignments[^2] : null;
            return previousAssignment?.UserId == userId;
        }

        private static IEnumerable<int> GetActiveStaffAssigneeIds(Ticket ticket)
        {
            var orderedAssignments = ticket.Assignments
                .OrderBy(a => a.AssignmentDate)
                .ThenBy(a => a.AssignmentId)
                .ToList();
            var latestAssignment = orderedAssignments.LastOrDefault();
            if (latestAssignment is null)
                return Enumerable.Empty<int>();

            var activeAssignments = new List<TicketUser> { latestAssignment };

            if (latestAssignment.AssignmentType == AssignmentType.FORWARDED_TO_TECHNICIAN && orderedAssignments.Count > 1)
                activeAssignments.Add(orderedAssignments[^2]);

            return activeAssignments
                .Where(a => a.User is not null && a.User.Role is Role.AGENT or Role.TECHNICIAN or Role.ADMINISTRATOR)
                .Select(a => a.UserId)
                .Distinct()
                .ToList();
        }

        // PB-56: validacija/upis attachmenta je centralizovana u AttachmentStorage.
        // US-14, US-30: Dohvata detalje tiketa uz provjeru pristupa prema roli
        public async Task<TicketDetailDto> GetTicketByIdAsync(int ticketId, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"                   => ticket.CreatorId == userId,
                "TECHNICIAN"               => ticket.Assignments.Any(a => a.UserId == userId),
                _                          => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Nemate pristup ovom tiketu.");

            var agentAssignment = ticket.Assignments
                .Where(a => a.AssignmentType != AssignmentType.FORWARDED_TO_TECHNICIAN
                         && a.User.Role != Role.TECHNICIAN)
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            var technicianAssignment = ticket.Assignments
                .Where(a => a.AssignmentType == AssignmentType.FORWARDED_TO_TECHNICIAN
                         || a.User.Role == Role.TECHNICIAN)
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            bool isStaff = role is "ADMINISTRATOR" or "AGENT" or "TECHNICIAN";

            return new TicketDetailDto
            {
                TicketId          = ticket.TicketId,
                CreatorId         = ticket.CreatorId,
                Title             = ticket.Title,
                Description       = ticket.Description,
                Status            = ticket.Status.ToString(),
                Priority          = ticket.Priority.ToString(),
                InternalPriority  = isStaff ? ticket.InternalPriority?.ToString() : null,
                ProblemCategory   = ticket.ProblemCategory.ToString(),
                CreatedDate       = ticket.CreatedDate,
                ClosedDate        = ticket.ClosedDate,
                ClosureRequestedDate = ticket.ClosureRequestedDate,
                ClosureRequestedById = ticket.ClosureRequestedById,
                ClosureRequestStatus = ticket.ClosureRequestStatus?.ToString(),
                ClosedById           = ticket.ClosedById,
                ClientName        = $"{ticket.Creator.FirstName} {ticket.Creator.LastName}",
                AssignedAgentName = agentAssignment is not null
                    ? $"{agentAssignment.User.FirstName} {agentAssignment.User.LastName}"
                    : technicianAssignment is not null
                        ? $"{technicianAssignment.User.FirstName} {technicianAssignment.User.LastName}"
                        : string.Empty,
                AssignedAgentId = agentAssignment?.UserId ?? technicianAssignment?.UserId,
                AssignedTechnicianName = technicianAssignment is not null
                    ? $"{technicianAssignment.User.FirstName} {technicianAssignment.User.LastName}"
                    : string.Empty,
                AssignedTechnicianId = technicianAssignment?.UserId,
                Attachments = ticket.Attachments.Select(a => new AttachmentDto
                {
                    AttachmentId = a.AttachmentId,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    Size = a.Size,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"/api/attachments/{a.AttachmentId}",
                    UploadedByUserId = a.UserId,
                    UploadedByName = a.User is null
                        ? string.Empty
                        : $"{a.User.FirstName} {a.User.LastName}".Trim()
                }).ToList()
            };
        }

        // PB-36 / US-60: Tehničar mijenja status tiketa koji mu je dodijeljen.
        // Dozvoljeni ciljni statusi za tehničara: OPEN, CLOSURE_REQUESTED.
        // Direktno zatvaranje (CLOSED) ide kroz client-confirm tok (request-closure / accept-closure),
        // pa ovdje nije dozvoljeno da tehničar postavi CLOSED.
        private static readonly TicketStatus[] TechnicianAllowedStatuses =
            { TicketStatus.OPEN, TicketStatus.CLOSURE_REQUESTED };

        public async Task UpdateTicketStatusAsync(int ticketId, TicketStatus newStatus, int userId, string role)
        {
            if (role != "TECHNICIAN")
                throw new UnauthorizedAccessException("Samo tehničar može mijenjati status tiketa.");

            if (!TechnicianAllowedStatuses.Contains(newStatus))
                throw new InvalidOperationException("Odabrani status nije dozvoljen za tehničara.");

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (ticket.Status == TicketStatus.CLOSED)
                throw new InvalidOperationException("Status zatvorenog tiketa se ne može mijenjati.");

            var latestAssignment = ticket.Assignments
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            if (latestAssignment?.UserId != userId)
                throw new UnauthorizedAccessException("Možete mijenjati status samo tiketa koji su vama dodijeljeni.");

            if (ticket.Status == newStatus)
                return;

            var previousStatus = ticket.Status;
            ticket.Status = newStatus;

            if (newStatus == TicketStatus.CLOSURE_REQUESTED)
            {
                ticket.ClosureRequestedDate = DateTime.Now;
                ticket.ClosureRequestedById = userId;
                ticket.ClosureRequestStatus = ClosureRequestStatus.PENDING;
            }
            else if (newStatus == TicketStatus.OPEN && previousStatus == TicketStatus.CLOSURE_REQUESTED)
            {
                ticket.ClosureRequestStatus = ClosureRequestStatus.REJECTED;
            }

            await _ticketRepository.UpdateAsync(ticket);

            await _notificationService.SendNotificationAsync(
                ticket.CreatorId,
                "Promjena statusa tiketa",
                $"Status vašeg tiketa \"{ticket.Title}\" je promijenjen na '{newStatus}'.",
            // US-14, US-30: Dohvata detalje tiketa uz provjeru pristupa prema roli
                NotificationType.STATUS_CHANGED,
                ticket.TicketId);
        }

        public async Task UpdateInternalPriorityAsync(int ticketId, InternalPriority priority, int userId, string role)
        {
            if (role is not ("ADMINISTRATOR" or "AGENT"))
                throw new UnauthorizedAccessException("Samo agenti i administratori mogu mijenjati interni prioritet.");

            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            ticket.InternalPriority = priority;
            await _ticketRepository.UpdateAsync(ticket);
        }

        public async Task CloseTicketAsync(int ticketId, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (ticket.Status == TicketStatus.CLOSED)
                throw new InvalidOperationException("Tiket je već zatvoren.");

            // Klijent može zatvoriti samo svoj tiket
            if (role == "CLIENT" && ticket.CreatorId != userId)
                throw new UnauthorizedAccessException("Nemate dozvolu za zatvaranje ovog tiketa.");

            // Agent/Admin/Technician mogu zatvoriti ako su dodijeljeni ili imaju ovlaštenja (ovdje dozvoljavamo svima sa Staff rolom radi jednostavnosti, uz validaciju u Controlleru)
            
            ticket.Status = TicketStatus.CLOSED;
            ticket.ClosedDate = DateTime.Now;
            ticket.ClosedById = userId;
            await _ticketRepository.UpdateAsync(ticket);

            if (role != "CLIENT")
                await _notificationService.SendNotificationAsync(
                    ticket.CreatorId,
                    "Tiket zatvoren",
                    $"Vaš tiket \"{ticket.Title}\" je zatvoren.",
                    NotificationType.TICKET_CLOSED,
                    ticket.TicketId);
        }

        public async Task RequestClosureAsync(int ticketId, int userId, string role)
        {
            if (role is not ("AGENT" or "TECHNICIAN" or "ADMINISTRATOR"))
                throw new UnauthorizedAccessException("Samo osoblje može zatražiti zatvaranje tiketa.");

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (ticket.Status != TicketStatus.OPEN)
                throw new InvalidOperationException("Zahtjev za zatvaranje se može poslati samo za otvorene tikete.");

            if (role is "AGENT" or "TECHNICIAN")
            {
                if (!IsActiveAssignee(ticket, userId))
                    throw new UnauthorizedAccessException("Možete zatražiti zatvaranje samo za tiket koji je vama dodijeljen.");
            }

            ticket.Status = TicketStatus.CLOSURE_REQUESTED;
            ticket.ClosureRequestedDate = DateTime.Now;
            ticket.ClosureRequestedById = userId;
            ticket.ClosureRequestStatus = ClosureRequestStatus.PENDING;

            await _ticketRepository.UpdateAsync(ticket);

            await _notificationService.SendNotificationAsync(
                ticket.CreatorId,
                "Promjena statusa tiketa",
                $"Status vašeg tiketa \"{ticket.Title}\" je promijenjen na 'Čeka zatvaranje'.",
                NotificationType.STATUS_CHANGED,
                ticket.TicketId);
        }

        public async Task AcceptClosureAsync(int ticketId, int userId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (ticket.CreatorId != userId)
                throw new UnauthorizedAccessException("Samo kreator tiketa može prihvatiti zatvaranje.");

            if (ticket.Status != TicketStatus.CLOSURE_REQUESTED)
                throw new InvalidOperationException("Ovaj tiket nema aktivan zahtjev za zatvaranje.");

            ticket.Status = TicketStatus.CLOSED;
            ticket.ClosedDate = DateTime.Now;
            ticket.ClosedById = userId;
            ticket.ClosureRequestStatus = ClosureRequestStatus.ACCEPTED;

            await _ticketRepository.UpdateAsync(ticket);

            foreach (var staffUserId in GetActiveStaffAssigneeIds(ticket))
            {
                await _notificationService.SendNotificationAsync(
                    staffUserId,
                    "Tiket zatvoren",
                    $"Klijent je prihvatio zatvaranje tiketa \"{ticket.Title}\".",
                    NotificationType.TICKET_CLOSED,
                    ticket.TicketId);
            }
        }

        public async Task RejectClosureAsync(int ticketId, int userId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (ticket.CreatorId != userId)
                throw new UnauthorizedAccessException("Samo kreator tiketa može odbiti zatvaranje.");

            if (ticket.Status != TicketStatus.CLOSURE_REQUESTED)
                throw new InvalidOperationException("Ovaj tiket nema aktivan zahtjev za zatvaranje.");

            ticket.Status = TicketStatus.OPEN;
            ticket.ClosureRequestStatus = ClosureRequestStatus.REJECTED;

            await _ticketRepository.UpdateAsync(ticket);

            foreach (var staffUserId in GetActiveStaffAssigneeIds(ticket))
            {
                await _notificationService.SendNotificationAsync(
                    staffUserId,
                    "Zatvaranje odbijeno",
                    $"Klijent je odbio zatvaranje tiketa \"{ticket.Title}\".",
                    NotificationType.STATUS_CHANGED,
                    ticket.TicketId);
            }
        }

        public async Task ForceCloseAsync(int ticketId, int userId, string role)
        {
            if (role is not ("AGENT" or "TECHNICIAN" or "ADMINISTRATOR"))
                throw new UnauthorizedAccessException("Nemate dozvolu za prisilno zatvaranje tiketa.");

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            if (role is "AGENT" or "TECHNICIAN" && !IsActiveAssignee(ticket, userId))
                throw new UnauthorizedAccessException("Možete prisilno zatvoriti samo tiket koji je vama dodijeljen.");

            if (ticket.Status != TicketStatus.CLOSURE_REQUESTED)
                throw new InvalidOperationException("Tiket mora biti u statusu 'Zahtjev za zatvaranje' da bi se mogao prisilno zatvoriti.");

            // Provjera 7 dana od zadnjeg komentara klijenta
            var lastClientComment = ticket.Comments
                .Where(c => c.Author != null && c.Author.Role == Role.CLIENT)
                .OrderByDescending(c => c.DateTime)
                .FirstOrDefault();

            DateTime referenceDate = lastClientComment?.DateTime ?? ticket.CreatedDate;

            if (DateTime.Now < referenceDate.AddDays(7))
                throw new InvalidOperationException("Mora proći 7 dana bez odgovora klijenta da bi se tiket prisilno zatvorio.");

            ticket.Status = TicketStatus.CLOSED;
            ticket.ClosedDate = DateTime.Now;
            ticket.ClosedById = userId;
            ticket.ClosureRequestStatus = ClosureRequestStatus.EXPIRED;

            await _ticketRepository.UpdateAsync(ticket);

            await _notificationService.SendNotificationAsync(
                ticket.CreatorId,
                "Tiket zatvoren",
                $"Vaš tiket \"{ticket.Title}\" je zatvoren.",
                NotificationType.TICKET_CLOSED,
                ticket.TicketId);
        }

        // PB-32: Lista tiketa filtrirana prema roli — AGENT/ADMIN vide sve, TECHNICIAN samo dodijeljene
        // assignedOnly=true: AGENT dobija samo tikete na kojima je dodijeljen
        public async Task<IEnumerable<MyTicketDto>> GetAllTicketsAsync(int userId, string role, bool assignedOnly = false)
        {
            IEnumerable<Ticket> tickets = role switch
            {
                "AGENT" when assignedOnly  => await _ticketRepository.GetByAssigneeIdAsync(userId),
                "ADMINISTRATOR" or "AGENT" => await _ticketRepository.GetAllAsync(),
                "TECHNICIAN"               => await _ticketRepository.GetByAssigneeIdAsync(userId),
                _                          => throw new UnauthorizedAccessException("Pristup nije dozvoljen.")
            };

            return tickets.Select(t => new MyTicketDto
            {
                TicketId        = t.TicketId,
                Title           = t.Title,
                Description     = t.Description,
                Status          = t.Status.ToString(),
                Priority        = t.Priority.ToString(),
                InternalPriority = t.InternalPriority?.ToString(),
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate,
                HasAssignment   = t.Assignments.Count > 0,
            });
        }

        // US-53: Otvoreni tiketi dodijeljeni agentu
        public async Task<IEnumerable<MyTicketDto>> GetOpenAssignedTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetOpenAssignedTicketsAsync(userId);

            return tickets.Select(t => new MyTicketDto
            {
                TicketId        = t.TicketId,
                Title           = t.Title,
                Description     = t.Description,
                Status          = t.Status.ToString(),
                Priority        = t.Priority.ToString(),
                InternalPriority = t.InternalPriority?.ToString(),
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate
            });
        }

        // US-54: Zatvoreni tiketi koji su bili dodijeljeni agentu
        public async Task<IEnumerable<MyTicketDto>> GetClosedAssignedTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetClosedAssignedTicketsAsync(userId);

            return tickets.Select(t => new MyTicketDto
            {
                TicketId        = t.TicketId,
                Title           = t.Title,
                Description     = t.Description,
                Status          = t.Status.ToString(),
                Priority        = t.Priority.ToString(),
                InternalPriority = t.InternalPriority?.ToString(),
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate
            });
        }

        // US-55, US-56: Vraća težine za scoring prema prioritetu tiketa —
        // visok prioritet favorizuje dostupnost, nizak prioritet favorizuje iskustvo
        private static (double experience, double rating, double availability) GetWeightsByPriority(Priority priority)
            => priority switch
            {
                Priority.LOW    => (0.6, 0.3, 0.1),
                Priority.MEDIUM => (0.5, 0.3, 0.2),
                Priority.HIGH   => (0.3, 0.2, 0.5),
                _               => (0.5, 0.3, 0.2)
            };

        // US-55, US-56: Izračunava relativni score za svakog agenta unutar kandidatskog skupa
        private static List<AgentScoreDto> CalculateAgentScores(
            IEnumerable<User> agents,
            ProblemCategory category,
            Priority priority)
        {
            var metrics = agents.Select(agent =>
            {
                var closedInCategory = agent.TicketAssignments
                    .Where(ta => ta.Ticket.Status == TicketStatus.CLOSED
                              && ta.Ticket.ProblemCategory == category)
                    .ToList();

                var resolved = closedInCategory.Count;
                var avgRating = closedInCategory.Any(ta => ta.Ticket.Rating != null)
                    ? closedInCategory
                        .Where(ta => ta.Ticket.Rating != null)
                        .Average(ta => (double)ta.Ticket.Rating!.RatingValue)
                    : 0.0;
                var openCount = agent.TicketAssignments
                    .Count(ta => ta.Ticket.Status == TicketStatus.OPEN);

                return new { Agent = agent, Resolved = resolved, AvgRating = avgRating, OpenCount = openCount };
            }).ToList();

            double maxResolved  = metrics.Max(m => (double)m.Resolved);
            double maxRating    = metrics.Max(m => m.AvgRating);
            double maxOpen      = metrics.Max(m => (double)m.OpenCount);

            var (wExperience, wRating, wAvailability) = GetWeightsByPriority(priority);

            return metrics.Select(m =>
            {
                double experienceScore   = maxResolved > 0 ? m.Resolved / maxResolved         : 0.0;
                double ratingScore       = maxRating   > 0 ? m.AvgRating / maxRating           : 0.0;
                double availabilityScore = maxOpen     > 0 ? (maxOpen - m.OpenCount) / maxOpen : 1.0;

                double finalScore = experienceScore   * wExperience
                                  + ratingScore       * wRating
                                  + availabilityScore * wAvailability;

                return new AgentScoreDto
                {
                    UserId             = m.Agent.UserId,
                    FullName           = $"{m.Agent.FirstName} {m.Agent.LastName}",
                    ScorePercent       = Math.Round(finalScore * 100, 1),
                    ResolvedInCategory = m.Resolved,
                    AvgRating          = Math.Round(m.AvgRating, 2),
                    OpenTickets        = m.OpenCount
                };
            })
            .OrderByDescending(a => a.ScorePercent)
            .ToList();
        }

        // US-55, US-56: Zajednička logika — validacija tiketa i vlasništva, pa izvršenje prosljeđivanja
        private async Task<TicketUser> ExecuteForwardAsync(int ticketId, int targetAgentId, int currentAgentId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            var currentAssignment = ticket.Assignments
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            if (currentAssignment?.UserId != currentAgentId)
                throw new UnauthorizedAccessException("Samo trenutni vlasnik tiketa može ga proslijediti.");

            var targetAgent = await _userRepository.GetAvailableAgentsForForwardingAsync(currentAgentId)
                .ContinueWith(t => t.Result.FirstOrDefault(a => a.UserId == targetAgentId))
                ?? throw new InvalidOperationException("Odabrani agent nije dostupan za prosljeđivanje.");

            var newAssignment = new TicketUser
            {
                TicketId       = ticketId,
                UserId         = targetAgent.UserId,
                TeamId         = targetAgent.TeamId ?? currentAssignment.TeamId,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.FORWARDED_TO_AGENT,
                Note           = "Prosljeđivanje tiketa od strane agenta"
            };

            if (targetAgent.TeamId.HasValue)
                ticket.TeamId = targetAgent.TeamId;

            await _ticketRepository.AddAssignmentAsync(newAssignment);

            await _notificationService.SendNotificationAsync(
                targetAgent.UserId,
                "Tiket je proslijeđen vama",
                $"Tiket \"{ticket.Title}\" je proslijeđen vama.",
                NotificationType.TICKET_FORWARDED,
                ticket.TicketId);

            await _commentService.AddSystemCommentAsync(
                ticketId,
                $"Tiket je proslijeđen agentu: {targetAgent.FirstName} {targetAgent.LastName}");

            await _notificationService.SendNotificationAsync(
                ticket.CreatorId,
                "Tiket proslijeđen",
                $"Vaš tiket \"{ticket.Title}\" je proslijeđen drugom agentu.",
                NotificationType.TICKET_FORWARDED,
                ticket.TicketId);

            return newAssignment;
        }

        // US-56: Vraća listu agenata sa score-ovima za ručni odabir
        public async Task<IEnumerable<AgentScoreDto>> GetAgentScoresAsync(int ticketId, int currentAgentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            var agents = await _userRepository.GetAvailableAgentsForForwardingAsync(currentAgentId);

            if (!agents.Any())
                return Enumerable.Empty<AgentScoreDto>();

            return CalculateAgentScores(agents, ticket.ProblemCategory, ticket.Priority);
        }

        // US-55: Automatski proslijedi tiket agentu s najvišim score-om
        public async Task<AgentScoreDto> AutoForwardTicketAsync(int ticketId, int currentAgentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            var agents = (await _userRepository.GetAvailableAgentsForForwardingAsync(currentAgentId)).ToList();

            if (agents.Count == 0)
                throw new InvalidOperationException("Nema dostupnih agenata za prosljeđivanje.");

            var scores = CalculateAgentScores(agents, ticket.ProblemCategory, ticket.Priority);
            var bestAgent = scores.First();

            await ExecuteForwardAsync(ticketId, bestAgent.UserId, currentAgentId);
            return bestAgent;
        }

        // US-56: Proslijedi tiket konkretnom odabranom agentu
        public async Task<AgentScoreDto> ForwardTicketToAgentAsync(int ticketId, int targetAgentId, int currentAgentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            var agents = (await _userRepository.GetAvailableAgentsForForwardingAsync(currentAgentId)).ToList();

            if (agents.Count == 0)
                throw new InvalidOperationException("Nema dostupnih agenata za prosljeđivanje.");

            var scores = CalculateAgentScores(agents, ticket.ProblemCategory, ticket.Priority);
            var targetScore = scores.FirstOrDefault(s => s.UserId == targetAgentId)
                ?? throw new InvalidOperationException("Odabrani agent nije dostupan za prosljeđivanje.");

            await ExecuteForwardAsync(ticketId, targetAgentId, currentAgentId);
            return targetScore;
        }

        // US-TechnicianForwarding: Proslijedi tiket tehničaru na lokaciji kreatora tiketa
        public async Task<AgentScoreDto> ForwardTicketToTechnicianAsync(int ticketId, int currentAgentId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId)
                ?? throw new KeyNotFoundException($"Tiket {ticketId} nije pronađen.");

            var location = ticket.Creator.Location;
            var currentAssignment = ticket.Assignments
                .OrderByDescending(a => a.AssignmentDate)
                .FirstOrDefault();

            if (currentAssignment?.UserId != currentAgentId)
                throw new UnauthorizedAccessException("Samo trenutni vlasnik tiketa može ga proslijediti.");

            // Dohvati sve tehničare na toj lokaciji
            var techniciansAtLocation = (await _userRepository.GetTechniciansByLocationAsync(location)).ToList();

            if (!techniciansAtLocation.Any())
                throw new InvalidOperationException("Nema dostupnog tehničara na toj lokaciji.");

            // Algoritam za dodjelu:
            // 1. Ako neki tehničar ima 0 otvorenih tiketa, dodijeli njemu
            // 2. Ako svi imaju, dodijeli onom s najmanje otvorenih tiketa
            
            // Moramo dohvatiti workload za svakog tehničara
            // Workload = broj otvorenih tiketa dodijeljenih tom tehničaru
            var techWorkloads = techniciansAtLocation.Select(tech => new
            {
                Technician = tech,
                OpenTicketsCount = tech.TicketAssignments.Count(ta => ta.Ticket.Status == TicketStatus.OPEN)
            }).ToList();

            var bestTech = techWorkloads
                .OrderBy(tw => tw.OpenTicketsCount)
                .First()
                .Technician;

            var newAssignment = new TicketUser
            {
                TicketId       = ticketId,
                UserId         = bestTech.UserId,
                TeamId         = bestTech.TeamId ?? currentAssignment.TeamId,
                AssignmentDate = DateTime.UtcNow,
                AssignmentType = AssignmentType.FORWARDED_TO_TECHNICIAN,
                Note           = $"Prosljeđivanje tehničaru na lokaciji {location}"
            };

            if (bestTech.TeamId.HasValue)
                ticket.TeamId = bestTech.TeamId;

            await _ticketRepository.AddAssignmentAsync(newAssignment);

            await _notificationService.SendNotificationAsync(
                bestTech.UserId,
                "Tiket je proslijeđen vama",
                $"Tiket \"{ticket.Title}\" je proslijeđen vama.",
                NotificationType.TICKET_FORWARDED,
                ticket.TicketId);

            await _commentService.AddSystemCommentAsync(
                ticketId,
                $"Tiket je proslijeđen tehničaru: {bestTech.FirstName} {bestTech.LastName}");

            await _notificationService.SendNotificationAsync(
                ticket.CreatorId,
                "Tiket proslijeđen",
                $"Vaš tiket \"{ticket.Title}\" je proslijeđen tehničaru.",
                NotificationType.TICKET_FORWARDED,
                ticket.TicketId);

            return new AgentScoreDto
            {
                UserId = bestTech.UserId,
                FullName = $"{bestTech.FirstName} {bestTech.LastName}",
                ScorePercent = 100 // Tehničar je "100% kompatibilan" jer je jedini izbor po lokaciji
            };
        }

        // US-25: Kreira tiket i automatski ga dodjeljuje agentu prema kategoriji
        public async Task<GetTicketDto> CreateTicketAsync(CreateTicketDto createTicketDto, int userId, IEnumerable<FileUploadDto>? attachments = null)
        {
            var team = await _teamRepository.GetBySpecializedCategoryAsync(createTicketDto.Type);

            var ticket = new Ticket
            {
                Title           = createTicketDto.Subject,
                Description     = createTicketDto.Description,
                CreatedDate     = DateTime.UtcNow,
                Status          = TicketStatus.OPEN,
                Priority        = createTicketDto.Priority,
                ProblemCategory = createTicketDto.Type,
                CreatorId       = userId,
                TeamId          = team?.TeamId
            };

            // PB-56: validaciju izvršavamo PRIJE kreiranja tiketa kako ne bismo ostavili siroče tikete u DB
            if (attachments is not null && attachments.Any())
            {
                AttachmentStorage.Validate(attachments, AttachmentStorage.MaxAttachmentsPerTicket);
            }

            await _ticketRepository.CreateAsync(ticket);

            if (attachments is not null && attachments.Any())
            {
                var (attachmentEntities, writtenPaths) =
                    AttachmentStorage.WriteFiles(attachments, ticket.TicketId, null, userId);

                try
                {
                    await _attachmentRepository.AddRangeAsync(attachmentEntities);
                    ticket.Attachments = attachmentEntities;
                }
                catch
                {
                    // US-80: ako DB save padne, počisti fajlove sa diska
                    AttachmentStorage.CleanupFiles(writtenPaths);
                    throw;
                }
            }

            string? assignedAgentName = null;
            string? assignmentMessage = null;

            if (team is null)
            {
                assignmentMessage = "Nema definisanih pravila dodjele za odabranu kategoriju.";
            }
            else
            {
                var agents = (await _userRepository.GetAvailableAgentsByTeamIdAsync(team.TeamId)).ToList();

                if (agents.Count == 0)
                {
                    assignmentMessage = "Nema dostupnih agenata. Tiket je označen kao Nedodijeljen.";
                }
                else
                {
                    // Sortiranje: prvo po broju dodijeljenih tiketa (ASC), pa po prosječnom prioritetu — load (ASC)
                    var bestAgent = agents
                        .OrderBy(a => a.TicketAssignments.Count)
                        .ThenBy(a => a.TicketAssignments.Count > 0
                            ? a.TicketAssignments.Average(ta => (int)ta.Ticket.Priority)
                            : 0.0)
                        .First();

                    await _ticketRepository.AddAssignmentAsync(new TicketUser
                    {
                        TicketId       = ticket.TicketId,
                        UserId         = bestAgent.UserId,
                        TeamId         = team.TeamId,
                        AssignmentDate = DateTime.UtcNow,
                        AssignmentType = AssignmentType.AUTOMATIC,
                        Note           = "Automatska dodjela prema kategoriji tiketa"
                    });

                    assignedAgentName = $"{bestAgent.FirstName} {bestAgent.LastName}";

                    await _notificationService.SendNotificationAsync(
                        bestAgent.UserId,
                        "Dodijeljen vam je tiket",
                        $"Tiket \"{ticket.Title}\" vam je dodijeljen.",
                        NotificationType.TICKET_ASSIGNED,
                        ticket.TicketId);
                }
            }

            return new GetTicketDto
            {
                TicketId         = ticket.TicketId,
                Title            = ticket.Title,
                Description      = ticket.Description,
                Status           = ticket.Status.ToString(),
                Priority         = ticket.Priority.ToString(),
                ProblemCategory  = ticket.ProblemCategory.ToString(),
                CreatedDate      = ticket.CreatedDate,
                ClosedDate       = ticket.ClosedDate,
                CreatorId        = ticket.CreatorId,
                TeamId           = ticket.TeamId,
                AssignedAgentName = assignedAgentName,
                AssignmentMessage = assignmentMessage
            };
        }
    }
}


