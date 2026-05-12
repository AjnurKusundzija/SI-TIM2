using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(
            ITicketRepository ticketRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _teamRepository   = teamRepository;
            _userRepository   = userRepository;
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

        // US-14, US-30: Dohvata detalje tiketa uz provjeru pristupa prema roli
        public async Task<TicketDetailDto> GetTicketByIdAsync(int ticketId, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);

            if (ticket is null)
                throw new KeyNotFoundException($"Ticket {ticketId} not found.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT"                   => ticket.CreatorId == userId,
                "TECHNICIAN"               => ticket.Assignments.Any(a => a.UserId == userId),
                _                          => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Access to this ticket is not allowed.");

            var agentAssignment = ticket.Assignments.FirstOrDefault();

            return new TicketDetailDto
            {
                TicketId          = ticket.TicketId,
                Title             = ticket.Title,
                Description       = ticket.Description,
                Status            = ticket.Status.ToString(),
                Priority          = ticket.Priority.ToString(),
                ProblemCategory   = ticket.ProblemCategory.ToString(),
                CreatedDate       = ticket.CreatedDate,
                ClosedDate        = ticket.ClosedDate,
                ClientName        = $"{ticket.Creator.FirstName} {ticket.Creator.LastName}",
                AssignedAgentName = agentAssignment is not null
                    ? $"{agentAssignment.User.FirstName} {agentAssignment.User.LastName}"
                    : string.Empty,
            };
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
                _                          => throw new UnauthorizedAccessException("Access not allowed.")
            };

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
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate
            });
        }

        // US-25: Kreira tiket i automatski ga dodjeljuje agentu prema kategoriji
        public async Task<GetTicketDto> CreateTicketAsync(CreateTicketDto createTicketDto, int userId)
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

            await _ticketRepository.CreateAsync(ticket);

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