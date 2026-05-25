using System.Collections.Generic;
using TelecomSupportSystem.BLL.DTOs;
using TelecomSupportSystem.BLL.DTOs.Packages;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public int? TeamId { get; set; }
        public string ExpertiseCategory { get; set; } = string.Empty;
        public IEnumerable<MyTicketDto> TicketHistory { get; set; } = new List<MyTicketDto>();
        public IEnumerable<PackageSummaryDto> ActivePackages { get; set; } = new List<PackageSummaryDto>();
    }
}
