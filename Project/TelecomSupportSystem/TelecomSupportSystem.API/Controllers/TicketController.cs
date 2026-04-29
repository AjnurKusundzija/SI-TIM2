using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.Services.Interfaces;
 
namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Svi endpointi zahtijevaju validan JWT
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
 
        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }
 
        // US-11: GET /api/ticket/my-tickets
        // Čita userId iz JWT claims-a — korisnik nikad ne može proslijediti
        // tuđi ID, što garantuje AC: "Sistem ne smije prikazivati tikete drugih korisnika"
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var tickets = await _ticketService.GetMyTicketsAsync(userId);
            return Ok(tickets);
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
 