using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.Services.Interfaces;
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
    }
}