using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.DTOs.Subscriptions;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    // PB-52 / US-77: Dodjela i ukidanje pretplata klijentima — samo administrator.
    [ApiController]
    [Route("api/clients/{clientId:int}/subscriptions")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class ClientSubscriptionController : ControllerBase
    {
        private readonly IClientSubscriptionService _service;

        public ClientSubscriptionController(IClientSubscriptionService service)
        {
            _service = service;
        }

        // GET /api/clients/{clientId}/subscriptions
        [HttpGet]
        public async Task<IActionResult> GetClientSubscriptions(int clientId)
        {
            try
            {
                var subscriptions = await _service.GetByClientIdAsync(clientId);
                return Ok(subscriptions);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Klijent nije pronađen." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/clients/{clientId}/subscriptions
        [HttpPost]
        public async Task<IActionResult> Assign(int clientId, [FromBody] AssignSubscriptionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!TryGetAdminId(out var adminId))
                return Unauthorized();

            try
            {
                var created = await _service.AssignAsync(clientId, dto, adminId);
                return CreatedAtAction(nameof(GetClientSubscriptions), new { clientId }, created);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PATCH /api/clients/{clientId}/subscriptions/{subscriptionId}/deactivate
        [HttpPatch("{subscriptionId:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int clientId, int subscriptionId)
        {
            if (!TryGetAdminId(out var adminId))
                return Unauthorized();

            try
            {
                var updated = await _service.DeactivateAsync(clientId, subscriptionId, adminId);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private bool TryGetAdminId(out int adminId)
        {
            adminId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return claim is not null && int.TryParse(claim, out adminId);
        }
    }
}
