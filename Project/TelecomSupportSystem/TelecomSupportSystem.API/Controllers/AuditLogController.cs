using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.AuditLogs;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    /// <summary>
    /// Kontroler za upravljanje audit logovima. Pristupačan samo ADMINISTRATORima.
    /// </summary>
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Dohvata audit logove sa paginacijom i filterima.
        /// </summary>
        /// <param name="page">Redni broj stranice (default 1)</param>
        /// <param name="pageSize">Broj stavki po stranici (default 20, max 100)</param>
        /// <param name="actionType">Filtriranje po tipu akcije</param>
        /// <param name="userId">Filtriranje po ID korisnika</param>
        /// <param name="entityType">Filtriranje po tipu entiteta</param>
        /// <param name="search">Pretraga po Description (LIKE)</param>
        /// <param name="dateFrom">Početni datum filtera</param>
        /// <param name="dateTo">Završni datum filtera</param>
        /// <returns>Paginirana lista audit logova</returns>
        /// <response code="200">Audit logovi uspješno dohvaćeni</response>
        /// <response code="401">Nema autentifikacije</response>
        /// <response code="403">Korisnik nema pristupa (nije administrator)</response>
        [HttpGet]
        public async Task<ActionResult<AuditLogResponseDto>> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? actionType = null,
            [FromQuery] int? userId = null,
            [FromQuery] string? entityType = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null
        )
        {
            // Validacija pageSize
            if (pageSize < 1) pageSize = 1;
            if (pageSize > 100) pageSize = 100;
            if (page < 1) page = 1;

            var filter = new AuditLogFilterDto
            {
                Search = search,
                ActionType = actionType,
                UserId = userId,
                EntityType = entityType,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Page = page,
                PageSize = pageSize
            };

            var result = await _auditLogService.GetAuditLogsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Dohvata detalje jednog audit loga, uključujući OldValue i NewValue.
        /// </summary>
        /// <param name="id">ID audit loga</param>
        /// <returns>Detalji audit loga</returns>
        /// <response code="200">Audit log uspješno dohvaćen</response>
        /// <response code="401">Nema autentifikacije</response>
        /// <response code="403">Korisnik nema pristupa (nije administrator)</response>
        /// <response code="404">Audit log nije pronađen</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<AuditLogDetailDto>> GetAuditLogDetail(int id)
        {
            var detail = await _auditLogService.GetAuditLogDetailAsync(id);
            if (detail == null)
                return NotFound(new { message = "Audit log nije pronađen." });

            return Ok(detail);
        }

        /// <summary>
        /// Dohvata listu svih dostupnih tipova akcija za dropdown.
        /// </summary>
        /// <returns>Lista stringova tipova akcija</returns>
        /// <response code="200">Tipovi akcija uspješno dohvaćeni</response>
        /// <response code="401">Nema autentifikacije</response>
        /// <response code="403">Korisnik nema pristupa (nije administrator)</response>
        [HttpGet("action-types")]
        public async Task<ActionResult<List<string>>> GetActionTypes()
        {
            var actionTypes = await _auditLogService.GetActionTypesAsync();
            return Ok(actionTypes);
        }

        /// <summary>
        /// Dohvata korisnike koji imaju barem jedan audit log zapis.
        /// </summary>
        /// <returns>Lista korisnika sa audit logovima</returns>
        /// <response code="200">Korisnici uspješno dohvaćeni</response>
        /// <response code="401">Nema autentifikacije</response>
        /// <response code="403">Korisnik nema pristupa (nije administrator)</response>
        [HttpGet("users")]
        public async Task<ActionResult<List<AuditLogUserDto>>> GetAuditLogUsers()
        {
            var users = await _auditLogService.GetAuditLogUsersAsync();
            return Ok(users);
        }
    }
}
