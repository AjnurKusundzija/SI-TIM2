using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TelecomSupportSystem.BLL.DTOs.Tickets;
using TelecomSupportSystem.BLL.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

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

        // US-53: GET /api/tickets/assigned/open
        // Agent vidi sve otvorene tikete koji su mu dodijeljeni
        [HttpGet("assigned/open")]
        public async Task<IActionResult> GetOpenAssignedTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            // Samo AGENT može pristupiti ovom endpointu
            if (role != "AGENT")
                return Forbid();

            var tickets = await _ticketService.GetOpenAssignedTicketsAsync(userId);
            return Ok(tickets);
        }

        // US-54: GET /api/tickets/assigned/closed
        // Agent vidi sve zatvorene tikete koji su mu bili dodijeljeni
        [HttpGet("assigned/closed")]
        public async Task<IActionResult> GetClosedAssignedTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            // Samo AGENT može pristupiti ovom endpointu
            if (role != "AGENT")
                return Forbid();

            var tickets = await _ticketService.GetClosedAssignedTicketsAsync(userId);
            return Ok(tickets);
        }

        // US-56: GET /api/tickets/{id}/forward/agents
        // Vraća sortiranu listu dostupnih agenata sa score-ovima za ručni odabir pri prosljeđivanju
        [HttpGet("{id:int}/forward/agents")]
        public async Task<IActionResult> GetAgentScores(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT")
                return Forbid();

            try
            {
                var agents = await _ticketService.GetAgentScoresAsync(id, userId);
                return Ok(agents);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // US-55: POST /api/tickets/{id}/forward/auto
        // Automatski proslijedi tiket agentu s najvišim score-om
        [HttpPost("{id:int}/forward/auto")]
        public async Task<IActionResult> AutoForwardTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT")
                return Forbid();

            try
            {
                var result = await _ticketService.AutoForwardTicketAsync(id, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        // US-56: POST /api/tickets/{id}/forward/agent
        // Proslijedi tiket konkretnom odabranom agentu
        [HttpPost("{id:int}/forward/agent")]
        public async Task<IActionResult> ForwardTicketToAgent(int id, [FromBody] ForwardToAgentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT")
                return Forbid();

            try
            {
                var result = await _ticketService.ForwardTicketToAgentAsync(id, dto.TargetAgentId, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        // US-TechnicianForwarding: POST /api/tickets/{id}/forward/technician
        // Automatski proslijedi tiket tehničaru na lokaciji kreatora tiketa
        [HttpPost("{id:int}/forward/technician")]
        public async Task<IActionResult> ForwardTicketToTechnician(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT")
                return Forbid();

            try
            {
                var result = await _ticketService.ForwardTicketToTechnicianAsync(id, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        // PB-62 / US-105: POST /api/tickets/{id}/self-assign
        // Agent jednim klikom preuzima nedodijeljeni tiket sebi
        [HttpPost("{id:int}/self-assign")]
        public async Task<IActionResult> SelfAssignTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            if (role != "AGENT")
                return Forbid();

            try
            {
                var result = await _ticketService.SelfAssignTicketAsync(id, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        // POST /api/tickets/{id}/internal-priority
        [HttpPost("{id:int}/internal-priority")]
        public async Task<IActionResult> UpdateInternalPriority(int id, [FromBody] UpdateInternalPriorityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                await _ticketService.UpdateInternalPriorityAsync(id, dto.Priority, userId, role);
                return Ok(new { message = "Interni prioritet uspješno ažuriran." });
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

        // PB-36 / US-60: POST /api/tickets/{id}/status
        // Tehničar mijenja status tiketa koji mu je dodijeljen
        [HttpPost("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTicketStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                await _ticketService.UpdateTicketStatusAsync(id, dto.Status, userId, role);
                return Ok(new { message = "Status tiketa je uspješno ažuriran." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
        }

        // POST /api/tickets/{id}/close
        [HttpPost("{id:int}/close")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                await _ticketService.CloseTicketAsync(id, userId, role);
                return Ok(new { message = "Tiket uspješno zatvoren." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
        }

        // POST /api/tickets/{id}/request-closure
        [HttpPost("{id:int}/request-closure")]
        public async Task<IActionResult> RequestClosure(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                await _ticketService.RequestClosureAsync(id, userId, role);
                return Ok(new { message = "Zahtjev za zatvaranje uspješno poslan." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
        }

        // POST /api/tickets/{id}/accept-closure
        [HttpPost("{id:int}/accept-closure")]
        public async Task<IActionResult> AcceptClosure(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                await _ticketService.AcceptClosureAsync(id, userId);
                return Ok(new { message = "Zatvaranje tiketa prihvaćeno." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
        }

        // POST /api/tickets/{id}/reject-closure
        [HttpPost("{id:int}/reject-closure")]
        public async Task<IActionResult> RejectClosure(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                await _ticketService.RejectClosureAsync(id, userId);
                return Ok(new { message = "Zatvaranje tiketa odbijeno." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
        }

        // POST /api/tickets/{id}/force-close
        [HttpPost("{id:int}/force-close")]
        public async Task<IActionResult> ForceClose(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId) || role is null)
                return Unauthorized();

            try
            {
                await _ticketService.ForceCloseAsync(id, userId, role);
                return Ok(new { message = "Tiket je prisilno zatvoren nakon isteka roka." });
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (InvalidOperationException ex) { return BadRequest(new { poruka = ex.Message }); }
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

        public class CreateTicketWithAttachmentsDto
        {
            [Required]
            public string Subject { get; set; } = string.Empty;

            [Required]
            public TelecomSupportSystem.DAL.Entities.Enums.ProblemCategory Type { get; set; }

            [Required]
            public string Description { get; set; } = string.Empty;

            [Required]
            public TelecomSupportSystem.DAL.Entities.Enums.Priority Priority { get; set; }

            public IFormFileCollection? Attachments { get; set; }
        }

        [HttpPost("attachments")]
        [RequestSizeLimit(52428800)]
        public async Task<IActionResult> CreateTicketWithAttachments([FromForm] CreateTicketWithAttachmentsDto createTicketDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var fileUploads = new List<TelecomSupportSystem.BLL.DTOs.Attachments.FileUploadDto>();
            if (createTicketDto.Attachments is not null)
            {
                foreach (var file in createTicketDto.Attachments)
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    fileUploads.Add(new TelecomSupportSystem.BLL.DTOs.Attachments.FileUploadDto
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType ?? "application/octet-stream",
                        Data = memoryStream.ToArray(),
                        Size = file.Length
                    });
                }
            }

            var ticket = await _ticketService.CreateTicketAsync(
                new CreateTicketDto
                {
                    Subject = createTicketDto.Subject,
                    Type = createTicketDto.Type,
                    Description = createTicketDto.Description,
                    Priority = createTicketDto.Priority
                },
                userId,
                fileUploads);

            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.TicketId }, ticket);
        }
    }
}
