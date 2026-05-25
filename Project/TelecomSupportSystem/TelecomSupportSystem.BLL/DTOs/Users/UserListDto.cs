using System.Collections.Generic;

namespace TelecomSupportSystem.BLL.DTOs.Users
{
    public class UserListDto
    {
        public IEnumerable<UserListItemDto> Users { get; set; } = new List<UserListItemDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
    }
}
