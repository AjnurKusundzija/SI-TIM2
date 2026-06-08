using TelecomSupportSystem.BLL.DTOs.Sla;
using TelecomSupportSystem.DAL.Entities;

namespace TelecomSupportSystem.BLL.Services.Interfaces
{
    public interface ISlaService
    {
        SlaInfoDto GetSlaInfo(Ticket ticket);
        int CountBreaches(IEnumerable<Ticket> tickets);
    }
}
