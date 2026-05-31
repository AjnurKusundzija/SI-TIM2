using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
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

        // US-102 / US-103: Posebna SignalR grupa za isporuku internih komentara —
        // samo osoblje (AGENT, TECHNICIAN, ADMINISTRATOR) može pristupiti. Pristup
        // dodatno provjeravamo iz JWT claim-ova; ako konekcija nije autentikovana
        // ili rola nije osoblje, zahtjev se ignorira (klijent nikad ne ulazi).
        [Authorize(Roles = "AGENT,TECHNICIAN,ADMINISTRATOR")]
        public async Task JoinTicketStaffGroup(string ticketId)
        {
            var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (role is null or "CLIENT") return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"ticket_{ticketId}_staff");
        }

        [Authorize(Roles = "AGENT,TECHNICIAN,ADMINISTRATOR")]
        public async Task LeaveTicketStaffGroup(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ticket_{ticketId}_staff");
        }
    }
}
