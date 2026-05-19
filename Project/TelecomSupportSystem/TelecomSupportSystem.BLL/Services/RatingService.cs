using TelecomSupportSystem.BLL.DTOs.Ratings;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.BLL.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ITicketRepository _ticketRepository;

        public RatingService(IRatingRepository ratingRepository, ITicketRepository ticketRepository)
        {
            _ratingRepository = ratingRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<RatingDto?> GetRatingForTicketAsync(int ticketId, int requestingUserId, string role)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null)
                throw new KeyNotFoundException($"Ticket {ticketId} not found.");

            bool hasAccess = role switch
            {
                "ADMINISTRATOR" or "AGENT" => true,
                "CLIENT" => ticket.CreatorId == requestingUserId,
                _ => false,
            };

            if (!hasAccess)
                throw new UnauthorizedAccessException("Access to this ticket is not allowed.");

            var rating = await _ratingRepository.GetByTicketIdAsync(ticketId);
            if (rating is null)
                return null;

            return MapToDto(rating);
        }

        public async Task<RatingDto> CreateRatingAsync(int ticketId, int userId, string role, CreateRatingDto dto)
        {
            if (role != "CLIENT")
                throw new UnauthorizedAccessException("Only clients can rate tickets.");

            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket is null)
                throw new KeyNotFoundException($"Ticket {ticketId} not found.");

            if (ticket.CreatorId != userId)
                throw new UnauthorizedAccessException("You can only rate your own tickets.");

            if (ticket.Status != TicketStatus.CLOSED)
                throw new InvalidOperationException("Only closed tickets can be rated.");

            var existing = await _ratingRepository.GetByTicketIdAsync(ticketId);
            if (existing is not null)
                throw new InvalidOperationException("This ticket has already been rated.");

            var rating = new Rating
            {
                TicketId = ticketId,
                UserId = userId,
                RatingValue = dto.RatingValue,
                RatingComment = dto.RatingComment ?? string.Empty,
                RatingDate = DateTime.UtcNow,
            };

            await _ratingRepository.CreateAsync(rating);

            return MapToDto(rating);
        }

        private static RatingDto MapToDto(Rating rating) => new()
        {
            RatingValue = rating.RatingValue,
            RatingComment = rating.RatingComment,
            RatingDate = rating.RatingDate,
            UserId = rating.UserId,
        };
    }
}
