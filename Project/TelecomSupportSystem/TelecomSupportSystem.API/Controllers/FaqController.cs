using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomSupportSystem.BLL.DTOs.Faq;
using TelecomSupportSystem.BLL.Services.Interfaces;

namespace TelecomSupportSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        // Read-only lista za sve prijavljene korisnike — vraća samo aktivne FAQ stavke
        [HttpGet]
        public async Task<IActionResult> GetFaqs()
        {
            var faqs = await _faqService.GetFaqsAsync();
            return Ok(faqs);
        }

        // PB-61 / US-104: Admin lista (uključuje neaktivne — koristi se u admin sekciji)
        [HttpGet("all")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> GetAllFaqs()
        {
            var faqs = await _faqService.GetAllFaqsAsync();
            return Ok(faqs);
        }

        // PB-61 / US-104: Admin kreira novu FAQ stavku
        [HttpPost]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> CreateFaq([FromBody] CreateFaqDto dto)
        {
            try
            {
                var created = await _faqService.CreateFaqAsync(dto);
                return CreatedAtAction(nameof(GetFaqs), new { id = created.FaqId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
        }

        // PB-61 / US-104: Admin uređuje postojeću FAQ stavku
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> UpdateFaq(int id, [FromBody] UpdateFaqDto dto)
        {
            try
            {
                var updated = await _faqService.UpdateFaqAsync(id, dto);
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { poruka = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // PB-61 / US-104: Admin briše FAQ stavku
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteFaq(int id)
        {
            try
            {
                await _faqService.DeleteFaqAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
