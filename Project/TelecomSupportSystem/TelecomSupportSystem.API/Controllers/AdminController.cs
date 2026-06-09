using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.Services.Interfaces;
using TelecomSupportSystem.DAL.Repositories.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class AdminController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ISlaService? _slaService;
        private readonly ITicketRepository? _ticketRepository;

        public AdminController(
            IReportService reportService,
            ISlaService? slaService = null,
            ITicketRepository? ticketRepository = null)
        {
            _reportService = reportService;
            _slaService = slaService;
            _ticketRepository = ticketRepository;
        }

        // PB-45 / US-71: GET /api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] string period = "month",
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            try
            {
                var dashboard = await _reportService.GetAdminDashboardAsync(period, from, to);
                return Ok(dashboard);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // US-116: GET /api/admin/sla-breach-count
        [HttpGet("sla-breach-count")]
        public async Task<IActionResult> GetSlaBreachCount()
        {
            if (_slaService is null || _ticketRepository is null)
                return Ok(new { breachCount = 0 });

            var tickets = await _ticketRepository.GetOpenTicketsAsync();
            var count = _slaService.CountBreaches(tickets);
            return Ok(new { breachCount = count });
        }
    }
}
