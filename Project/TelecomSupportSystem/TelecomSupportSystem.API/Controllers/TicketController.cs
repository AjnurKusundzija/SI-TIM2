using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize] // Svi endpointi zahtijevaju validan JWT
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // PB-32: GET /api/tickets?assignedOnly=true
        // ADMINISTRATOR i AGENT vide sve tikete, TECHNICIAN vidi samo dodijeljene, CLIENT dobija 403.
        // assignedOnly=true (opcionalno): AGENT vidi samo tikete na kojima je dodijeljen
        [HttpGet]
        public async Task<IActionResult> GetAllTickets([FromQuery] bool assignedOnly = false)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                var tickets = await _ticketService.GetAllTicketsAsync(userId, role, assignedOnly);
                return Ok(tickets);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // US-11: GET /api/mytickets
        // Čita userId iz JWT claims-a — korisnik nikad ne može proslijediti
        // tuđi ID, što garantuje AC: "Sistem ne smije prikazivati tikete drugih korisnika"
        [HttpGet("/api/mytickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var tickets = await _ticketService.GetMyTicketsAsync(userId);
            return Ok(tickets);
        }

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
