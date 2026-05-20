using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class AdminController : ControllerBase
    {
        private readonly IReportService _reportService;

        public AdminController(IReportService reportService)
        {
            _reportService = reportService;
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
    }
}
