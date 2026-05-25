using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.DTOs.Packages;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    // PB-52 / US-76: Administracija kataloga paketa.
    // Konflikt sa postojećim PackageController-om je izbjegnut korištenjem različitih HTTP
    // metoda (GET ostaje za klijentski view, POST/PUT/DELETE/PATCH su admin operacije).
    [ApiController]
    [Route("api/packages")]
    [Authorize(Roles = "ADMINISTRATOR")]
    public class PackageCatalogController : ControllerBase
    {
        private readonly ICatalogPackageService _service;

        public PackageCatalogController(ICatalogPackageService service)
        {
            _service = service;
        }

        // GET /api/packages/catalog — lista svih paketa (admin view).
        [HttpGet("catalog")]
        public async Task<IActionResult> GetCatalog()
        {
            var packages = await _service.GetCatalogAsync();
            return Ok(packages);
        }

        // GET /api/packages/catalog/active — lista samo aktivnih paketa.
        // Koristi se prilikom dodjele klijentu (US-77) — dropdown opcije.
        [HttpGet("catalog/active")]
        public async Task<IActionResult> GetActiveCatalog()
        {
            var packages = await _service.GetActiveCatalogAsync();
            return Ok(packages);
        }

        // POST /api/packages
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCatalogPackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto, GetUserId());
                return CreatedAtAction(nameof(GetCatalog), new { id = created.CatalogPackageId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/packages/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCatalogPackageDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto, GetUserId());
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Paket nije pronađen." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/packages/{id} — blokira brisanje ako postoje aktivne pretplate (409).
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Paket nije pronađen." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PATCH /api/packages/{id}/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePackageStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateStatusAsync(id, dto.Status, GetUserId());
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Paket nije pronađen." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int? GetUserId()
        {
            var raw = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
