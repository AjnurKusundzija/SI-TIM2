using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // US-11: GET /api/ticket/my-tickets
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var tickets = await _ticketService.GetMyTicketsAsync(userId);
            return Ok(tickets);
        }

        // US-29: GET /api/ticket?page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value
                    ?? User.FindFirst("role")?.Value;

            if (role != "AGENT" && role != "ADMINISTRATOR")
                return Forbid();

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var result = await _ticketService.GetAllTicketsAsync(page, pageSize);
            return Ok(result);
        }

        // US-30: GET /api/ticket/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketDetail(int id)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value
                    ?? User.FindFirst("role")?.Value;

            if (role != "AGENT" && role != "ADMINISTRATOR")
                return Forbid();

            var ticket = await _ticketService.GetTicketDetailAsync(id);
            if (ticket is null)
                return NotFound();

            return Ok(ticket);
        // US-14, US-30: GET /api/ticket/{id}
        // CLIENT vidi samo vlastite tikete, AGENT/TECHNICIAN samo dodijeljene, ADMINISTRATOR sve.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id, userId, role);
                return Ok(ticket);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // PB-22: POST /api/ticket
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto createTicketDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var ticket = await _ticketService.CreateTicketAsync(createTicketDto, userId);
            return CreatedAtAction(nameof(GetMyTickets), new { }, ticket);
        }
    }
}
