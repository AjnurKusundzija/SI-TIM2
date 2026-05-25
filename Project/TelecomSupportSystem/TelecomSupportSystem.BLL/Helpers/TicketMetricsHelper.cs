using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;
using Role = TelecomSupportSystem.DAL.Entities.Enums.Role;

namespace TelecomSupportSystem.BLL.Helpers
{
    public static class TicketMetricsHelper
    {
        public static double? CalculateAvgFirstResponseMinutes(IEnumerable<Ticket> tickets)
        {
            var minutes = tickets
                .Select(GetFirstResponseMinutes)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .ToList();

            return minutes.Count > 0 ? minutes.Average() : null;
        }

        public static double? GetFirstResponseMinutes(Ticket ticket)
        {
            var firstStaffComment = ticket.Comments
                .Where(c => c.Author != null && c.Author.Role != Role.CLIENT)
                .OrderBy(c => c.DateTime)
                .FirstOrDefault();

            if (firstStaffComment == null)
                return null;

            return (firstStaffComment.DateTime - ticket.CreatedDate).TotalMinutes;
        }

        public static double? CalculateAvgResolutionHours(IEnumerable<Ticket> tickets)
        {
            var hours = tickets
                .Where(t => t.Status == TicketStatus.CLOSED && t.ClosedDate.HasValue)
                .Select(t => (t.ClosedDate!.Value - t.CreatedDate).TotalHours)
                .ToList();

            return hours.Count > 0 ? hours.Average() : null;
        }

        public static double? CalculateAvgRating(IEnumerable<Ticket> tickets)
        {
            var ratings = tickets
                .Where(t => t.Rating != null)
                .Select(t => (double)t.Rating!.RatingValue)
                .ToList();

            return ratings.Count > 0 ? ratings.Average() : null;
        }
    }
}
