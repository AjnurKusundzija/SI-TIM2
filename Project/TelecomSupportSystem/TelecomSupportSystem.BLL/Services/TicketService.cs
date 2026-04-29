using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
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
                Status          = t.Status.ToString(),
                Priority        = t.Priority.ToString(),
                ProblemCategory = t.ProblemCategory.ToString(),
                CreatedDate     = t.CreatedDate,
                ClosedDate      = t.ClosedDate
            });
        }

        // PB-22: Kreira novi tiket
        public async Task<GetTicketDto> CreateTicketAsync(CreateTicketDto createTicketDto, int userId)
        {
            var ticket = new Ticket
            {
                Title = createTicketDto.Subject,
                Description = createTicketDto.Description,
                CreatedDate = DateTime.UtcNow,
                Status = DAL.Entities.Enums.TicketStatus.OPEN,
                Priority = createTicketDto.Priority,
                ProblemCategory = createTicketDto.Type,
                CreatorId = userId
            };

            await _ticketRepository.CreateAsync(ticket);

            return new GetTicketDto
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                ProblemCategory = ticket.ProblemCategory.ToString(),
                CreatedDate = ticket.CreatedDate,
                ClosedDate = ticket.ClosedDate,
                CreatorId = ticket.CreatorId,
                TeamId = ticket.TeamId
            };
        }
    }
}