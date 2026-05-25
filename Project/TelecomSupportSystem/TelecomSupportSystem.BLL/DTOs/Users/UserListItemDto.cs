using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UserListItemDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public string ExpertiseCategory { get; set; } = string.Empty;
    }
}
