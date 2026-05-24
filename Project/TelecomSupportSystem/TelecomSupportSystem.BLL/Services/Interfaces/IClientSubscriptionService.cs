using System.Collections.Generic;
using System.Threading.Tasks;
using TelecomSupportSystem.BLL.DTOs.Subscriptions;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface IClientSubscriptionService
    {
        // US-77
        Task<IEnumerable<ClientSubscriptionDto>> GetByClientIdAsync(int clientId);

        // US-77 — adminId iz JWT-a, koristi se za audit log
        Task<ClientSubscriptionDto> AssignAsync(int clientId, AssignSubscriptionDto dto, int adminId);

        // US-77
        Task<ClientSubscriptionDto> DeactivateAsync(int clientId, int subscriptionId, int adminId);
    }
}
