using TelecomSupportSystem.DAL.Entities;
using TelecomSupportSystem.DAL.Entities.Enums;

namespace TelecomSupportSystem.DAL.Repositories.Interfaces
{
    public interface ITeamRepository
    {
        Task<Team?> GetBySpecializedCategoryAsync(ProblemCategory category);
    }
}
