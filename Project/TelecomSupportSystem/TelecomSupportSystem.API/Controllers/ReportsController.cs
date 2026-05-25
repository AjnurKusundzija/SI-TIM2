using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.DTOs.Reports;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // PB-45 / US-83: POST /api/reports/generate — on-demand izvještaj
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport([FromBody] ReportRequestDto request)
        {
            try
            {
                var result = await _reportService.GenerateReportAsync(
                    request.ReportType,
                    request.Period,
                    request.From,
                    request.To);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
