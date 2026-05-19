using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    [Authorize] // US-6/US-7: Svaki prijavljeni korisnik može vidjeti svoje pakete
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        // US-6: GET /api/packages
        // Čita userId iz JWT-a — korisnik nikad ne može tražiti tuđe pakete.
        [HttpGet]
        public async Task<IActionResult> GetMyPackages()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var packages = await _packageService.GetMyPackagesAsync(userId);
            return Ok(packages);
        }

        // US-7: GET /api/packages/{id}
        // Paket mora pripadati prijavljenom korisniku, u suprotnom → 403.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var package = await _packageService.GetPackageByIdAsync(id, userId);
                return Ok(package);
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
    }
}
