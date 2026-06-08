using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Packages;
using TelecomSupportSystem.BLL.DTOs.Users;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPackageService _packageService;
        private readonly ITeamRepository _teamRepository;
        private readonly IAuditLogService? _auditLogService;
        private readonly ITicketService _ticketService;
        private readonly INotificationService _notificationService;

        public UserService(ITicketRepository ticketRepository, IUserRepository userRepository, IPackageService packageService, ITeamRepository teamRepository, ITicketService ticketService, INotificationService notificationService, IAuditLogService? auditLogService = null)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _packageService = packageService;
            _teamRepository = teamRepository;
            _auditLogService = auditLogService;
            _ticketService = ticketService;
            _notificationService = notificationService;
        }

        public async Task<AgentStatisticsDto> GetMyStatisticsAsync(int userId, string role)
        {
            var tickets = await _ticketRepository.GetAssignedTicketsForStatsAsync(userId);
            var ticketList = tickets.ToList();

            var openCount = ticketList.Count(t => t.Status == TicketStatus.OPEN);
            var closedCount = ticketList.Count(t => t.Status == TicketStatus.CLOSED);
            var pendingCount = ticketList.Count(t => t.Status == TicketStatus.CLOSURE_REQUESTED);

            // Prosječno vrijeme prvog odgovora: od kreiranja tiketa do prve poruke
            // bilo kojeg non-CLIENT korisnika (standardna helpdesk metrika)
            var firstResponseMinutes = ticketList
                .Select(t =>
                {
                    var firstStaffComment = t.Comments
                            .Where(c => c.Author != null && c.Author.Role != Role.CLIENT)
                        .OrderBy(c => c.DateTime)
                        .FirstOrDefault();
                    if (firstStaffComment == null) return (double?)null;
                    return (firstStaffComment.DateTime - t.CreatedDate).TotalMinutes;
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            double? avgFirstResponse = firstResponseMinutes.Count > 0
                ? firstResponseMinutes.Average()
                : null;

            // Prosječno vrijeme rješavanja: od kreiranja do zatvaranja
            var resolutionHours = ticketList
                .Where(t => t.Status == TicketStatus.CLOSED && t.ClosedDate.HasValue)
                .Select(t => (t.ClosedDate!.Value - t.CreatedDate).TotalHours)
                .ToList();

            double? avgResolution = resolutionHours.Count > 0
                ? resolutionHours.Average()
                : null;

            // Prosječna ocjena — samo za agente
            double? avgRating = null;
            if (role == "AGENT")
            {
                var ratings = ticketList
                    .Where(t => t.Rating != null)
                    .Select(t => (double)t.Rating!.RatingValue)
                    .ToList();
                avgRating = ratings.Count > 0 ? ratings.Average() : null;
            }

            return new AgentStatisticsDto
            {
                OpenTicketsCount = openCount,
                ClosedTicketsCount = closedCount,
                PendingClosureCount = pendingCount,
                AvgFirstResponseMinutes = avgFirstResponse,
                AvgResolutionHours = avgResolution,
                AvgRating = avgRating
            };
        }

        public async Task<IEnumerable<RecentTicketDto>> GetRecentAssignedTicketsAsync(int userId)
        {
            var tickets = await _ticketRepository.GetRecentAssignedTicketsAsync(userId, 5);

            return tickets.Select(t => new RecentTicketDto
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                LastActivityDate = t.Comments.Any()
                    ? t.Comments.Max(c => c.DateTime)
                    : t.CreatedDate
            });
        }

        public async Task<UserProfileDto> GetMyProfileAsync(int userId)
        {
            return await GetUserProfileAsync(userId, userId, "CLIENT");
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId, int requestingUserId, string role)
        {
            if (role == "CLIENT" && userId != requestingUserId)
                throw new UnauthorizedAccessException("Nemate pristup ovom profilu.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            var tickets = await _ticketRepository.GetByCreatorIdAsync(userId);
            var packages = await _packageService.GetMyPackagesAsync(userId);

            return new UserProfileDto
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                Location = user.Location.ToString(),
                AccountStatus = user.AccountStatus.ToString(),
                TeamId = user.TeamId,
                ExpertiseCategory = user.Team?.SpecializedCategory?.ToString() ?? "",
                Availability = user.AvailabilityStatus?.ToString(),
                TicketHistory = tickets.Select(t => new MyTicketDto
                {
                    TicketId = t.TicketId,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    ProblemCategory = t.ProblemCategory.ToString(),
                    CreatedDate = t.CreatedDate,
                    ClosedDate = t.ClosedDate,
                    InternalPriority = t.InternalPriority?.ToString()
                }).ToList(),
                ActivePackages = packages.ToList()
            };
        }

        public async Task UpdateEmailAsync(int userId, UpdateEmailDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            var normalizedEmail = dto.Email.Trim();
            var existing = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (existing != null && existing.UserId != userId)
                throw new InvalidOperationException("Email adresa je već zauzeta.");

            user.Email = normalizedEmail;
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdatePasswordAsync(int userId, UpdatePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Pogrešna trenutna lozinka.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task CreateUserAsync(CreateUserDto dto, string currentRole, int? currentUserId = null, string? currentUserEmail = null)
        {
            if (currentRole != "ADMINISTRATOR")
                throw new UnauthorizedAccessException("Samo administratori mogu kreirati korisnike.");

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Email adresa je već zauzeta.");

            if (!string.IsNullOrEmpty(dto.Phone) &&
                !System.Text.RegularExpressions.Regex.IsMatch(dto.Phone, @"^\+387[0-9]{8,9}$"))
                throw new ArgumentException("Broj telefona mora biti u međunarodnom formatu (npr. +38761234567).");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Location = dto.Location ?? Location.SARAJEVO, // Default if null for role client/tech
                AccountStatus = AccountStatus.ACTIVE,
                Username = dto.FirstName + "."+ dto.LastName, //username auto fill
                TeamId = dto.Role == Role.AGENT ? dto.TeamId : null,
                AvailabilityStatus = (dto.Role == Role.AGENT || dto.Role == Role.TECHNICIAN) ? AvailabilityStatus.AVAILABLE : null
            };

            await _userRepository.CreateAsync(user);

            if (_auditLogService is not null)
            {
                await _auditLogService.LogAsync(
                    AuditActionType.USER_CREATED,
                    "User",
                    user.UserId.ToString(),
                    $"Korisnik {user.Email} kreiran",
                    userId: currentUserId,
                    newValue: new { firstName = user.FirstName, lastName = user.LastName, email = user.Email, role = user.Role.ToString() });
            }
        }

        public async Task UpdateUserDetailsAsync(int targetUserId, UpdateUserDetailsDto dto, string currentRole, int? currentUserId = null)
        {
            var user = await _userRepository.GetByIdAsync(targetUserId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (currentRole != "ADMINISTRATOR" && currentRole != "AGENT")
                throw new UnauthorizedAccessException("Nemate permisije za ažuriranje ovog korisnika.");

            // Agent can only edit Client
            if (currentRole == "AGENT" && user.Role != Role.CLIENT && user.Role != Role.TECHNICIAN)
                throw new UnauthorizedAccessException("Agenti mogu ažurirati samo klijente i tehničare.");

            var oldValue = new
            {
                firstName = user.FirstName,
                lastName = user.LastName,
                phone = user.Phone,
                location = user.Location.ToString(),
                teamId = user.TeamId
            };

            if (!string.IsNullOrEmpty(dto.Phone) &&
                !System.Text.RegularExpressions.Regex.IsMatch(dto.Phone, @"^\+387[0-9]{8,9}$"))
                throw new ArgumentException("Broj telefona mora biti u međunarodnom formatu (npr. +38761234567).");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            
            if (dto.Location.HasValue)
                user.Location = dto.Location.Value;

            if (user.Role == Role.AGENT && dto.TeamId.HasValue)
                user.TeamId = dto.TeamId.Value;

            await _userRepository.UpdateAsync(user);

            if (_auditLogService is not null)
            {
                await _auditLogService.LogAsync(
                    AuditActionType.USER_UPDATED,
                    "User",
                    user.UserId.ToString(),
                    $"Korisnik {user.Email} ažuriran",
                    userId: currentUserId,
                    oldValue: oldValue,
                    newValue: new
                    {
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        phone = user.Phone,
                        location = user.Location.ToString(),
                        teamId = user.TeamId
                    });
            }
        }

        public async Task ChangeUserStatusAsync(int targetUserId, bool isActive, string currentRole, int currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(targetUserId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            if (targetUserId == currentUserId)
                throw new InvalidOperationException("Ne možete promijeniti status vlastitog naloga.");

            if (currentRole != "ADMINISTRATOR" && currentRole != "AGENT")
                throw new UnauthorizedAccessException("Nemate permisije.");

            if (currentRole == "AGENT" && user.Role != Role.CLIENT)
                throw new UnauthorizedAccessException("Agenti mogu deaktivirati samo klijente.");

            if (!isActive && (user.Role == Role.AGENT || user.Role == Role.TECHNICIAN))
            {
                var assignedTickets = await _ticketRepository.GetAssignedTicketsForStatsAsync(targetUserId);
                if (assignedTickets.Any(t => t.Status == TicketStatus.OPEN))
                {
                    throw new InvalidOperationException("Korisnik ima otvorene tikete. Potrebno ih je prvo preusmjeriti.");
                }
            }

            var oldStatus = user.AccountStatus;
            user.AccountStatus = isActive ? AccountStatus.ACTIVE : AccountStatus.INACTIVE;
            await _userRepository.UpdateAsync(user);

            if (_auditLogService is not null && oldStatus != user.AccountStatus)
            {
                var actor = await _userRepository.GetByIdAsync(currentUserId);
                var actorEmail = actor?.Email ?? currentRole;
                await _auditLogService.LogAsync(
                    isActive ? AuditActionType.USER_REACTIVATED : AuditActionType.USER_DEACTIVATED,
                    "User",
                    user.UserId.ToString(),
                    isActive
                        ? $"Korisnik {user.Email} reaktiviran od strane {actorEmail}"
                        : $"Korisnik {user.Email} deaktiviran od strane {actorEmail}",
                    userId: currentUserId,
                    oldValue: new { accountStatus = oldStatus.ToString() },
                    newValue: new { accountStatus = user.AccountStatus.ToString() });
            }
        }

        public async Task<UserListDto> GetUsersPaginatedAsync(string currentRole, string? roleFilter, string? statusFilter, string? availabilityFilter, string? search, string? location, int page, int pageSize)
        {
            if (currentRole != "ADMINISTRATOR" && currentRole != "AGENT")
                throw new UnauthorizedAccessException("Nemate permisije.");

            Role? role = null;
            if (!string.IsNullOrEmpty(roleFilter) && Enum.TryParse<Role>(roleFilter, true, out var parsedRole))
                role = parsedRole;

            AccountStatus? status = null;
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<AccountStatus>(statusFilter, true, out var parsedStatus))
                status = parsedStatus;

            TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus? availability = null;
            if (!string.IsNullOrEmpty(availabilityFilter) && Enum.TryParse<TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus>(availabilityFilter, true, out var parsedAvailability))
                availability = parsedAvailability;

            Location? loc = null;
            if (!string.IsNullOrEmpty(location) && Enum.TryParse<Location>(location, true, out var parsedLoc))
                loc = parsedLoc;

            var (users, totalCount) = await _userRepository.GetUsersPaginatedAsync(role, status, availability, search, loc, page, pageSize);
            var items = new List<UserListItemDto>();
            foreach (var u in users)
            {
                var openTickets = await _ticketRepository.GetOpenAssignedTicketsAsync(u.UserId);
                items.Add(new UserListItemDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Location = u.Location.ToString(),
                    Role = u.Role.ToString(),
                    AccountStatus = u.AccountStatus.ToString(),
                    ExpertiseCategory = u.Team?.SpecializedCategory?.ToString() ?? "",
                    Availability = u.AvailabilityStatus?.ToString(),
                    OpenAssignedTicketsCount = openTickets.Count()
                });
            }

            return new UserListDto
            {
                Users = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task SetAvailabilityAsync(int userId, string availability, string role, int actingUserId)
        {
            if (role != "ADMINISTRATOR" && role != "AGENT" && role != "TECHNICIAN")
                throw new UnauthorizedAccessException("Nemate permisije.");

            if (!Enum.TryParse<TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus>(availability, true, out var parsedAvailability))
                throw new InvalidOperationException("Neispravan status dostupnosti.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Korisnik nije pronađen.");

            // Only admins or the user themself can change availability
            if (role != "ADMINISTRATOR" && actingUserId != userId)
                throw new UnauthorizedAccessException("Možete mijenjati samo vlastiti status.");

            if (user.Role != Role.AGENT && user.Role != Role.TECHNICIAN)
                throw new InvalidOperationException("Samo agenti i tehničari imaju status dostupnosti.");

            var oldAvailability = user.AvailabilityStatus;
            user.AvailabilityStatus = parsedAvailability;
            await _userRepository.UpdateAsync(user);

            if (_auditLogService is not null && oldAvailability != user.AvailabilityStatus)
            {
                await _auditLogService.LogAsync(
                    AuditActionType.USER_UPDATED,
                    "User",
                    user.UserId.ToString(),
                    $"Korisnik {user.Email} promijenio status dostupnosti na {user.AvailabilityStatus}",
                    userId: actingUserId,
                    oldValue: new { availability = oldAvailability?.ToString() },
                    newValue: new { availability = user.AvailabilityStatus.ToString() });
            }

            // If user becomes UNAVAILABLE, reassign open tickets
            if (user.AvailabilityStatus == TelecomSupportSystem.DAL.Entities.Enums.AvailabilityStatus.UNAVAILABLE)
            {
                var openTickets = await _ticketRepository.GetOpenAssignedTicketsAsync(userId);
                foreach (var t in openTickets)
                {
                    try
                    {
                        await _ticketService.AutoForwardTicketAsync(t.TicketId, userId);
                    }
                    catch
                    {
                        // If no available agents, create notification for admins (handled below)
                    }
                }
            }

            // Notify administrators about the change so their UI can refresh in real-time
            var (admins, _) = await _userRepository.GetUsersPaginatedAsync(Role.ADMINISTRATOR, AccountStatus.ACTIVE, null, null, null, 1, 1000);
            foreach (var a in admins)
            {
                await _notificationService.SendNotificationAsync(
                    a.UserId,
                    "Promjena statusa agenta",
                    $"Agent {user.FirstName} {user.LastName} promijenio je status dostupnosti na {user.AvailabilityStatus}.",
                    NotificationType.STATUS_CHANGED);
            }

            // Notify the user themself
            await _notificationService.SendNotificationAsync(
                user.UserId,
                "Vaš status dostupnosti je promijenjen",
                $"Vaš status dostupnosti je sada: {user.AvailabilityStatus}.",
                NotificationType.STATUS_CHANGED);
        }

        public async Task<IEnumerable<TelecomSupportSystem.BLL.DTOs.Teams.TeamDto>> GetAgentTeamsAsync()
        {
            var teams = await _teamRepository.GetAgentTeamsAsync();
            return teams.Select(t => new TelecomSupportSystem.BLL.DTOs.Teams.TeamDto
            {
                TeamId = t.TeamId,
                TeamName = t.TeamName,
                SpecializedCategory = t.SpecializedCategory?.ToString() ?? ""
            });
        }
    }
}
