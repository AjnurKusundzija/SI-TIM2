using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TelecomSupportSystem.API.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinTicketGroup(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
        }

        public async Task LeaveTicketGroup(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket_{ticketId}");
        }
    }
}
