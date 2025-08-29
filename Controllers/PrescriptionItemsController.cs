using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.Services;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionItemsController : ControllerBase
    {
        private readonly IPrescriptionItemService _service;
        private readonly IMapper _mapper;

        public PrescriptionItemsController(IPrescriptionItemService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("GetPrescriptionItems")]
        public async Task<ActionResult> GetPrescriptionItems([FromQuery] PrescriptionITemFilterRequest? filterRequest, int page = 1)
        {
            if (page < 1) page = 1;

            var (items, totalPages) = await _service.GetPrescriptionItemsAsync(filterRequest, page);

            var result = new
            {
                Pagination = new
                {
                    TotalNumberOfPages = totalPages,
                    CurrentPage = page
                },
                Returns = new
                {
                    MedicalName = filterRequest?.MedicalName,
                    PrescriptionId = filterRequest?.PresctiptionId,
                    PrescriptionItems = items
                }
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionItem>> GetPrescriptionItem(int id)
        {
            var item = await _service.GetPrescriptionItemByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Prescription item not found." });

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<PrescriptionItem>> PostPrescriptionItem(PrescriptionItem item)
        {
            var success = await _service.CreatePrescriptionItemAsync(item);
            if (!success)
                return StatusCode(500, new { message = "Failed to save prescription item. Database error." });

            return CreatedAtAction(nameof(GetPrescriptionItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescriptionItem(int id, PrescriptionItem item)
        {
            if (id != item.Id)
                return BadRequest(new { message = "ID mismatch." });

            var success = await _service.UpdatePrescriptionItemAsync(item);
            if (!success)
            {
                var exists = await _service.GetPrescriptionItemByIdAsync(id);
                if (exists == null)
                    return NotFound(new { message = "Prescription item not found." });
                return StatusCode(409, new { message = "Concurrency conflict occurred while updating. Please retry." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionItem(int id)
        {
            var success = await _service.DeletePrescriptionItemAsync(id);
            if (!success)
                return NotFound(new { message = "Prescription item not found or failed to delete." });

            return NoContent();
        }
    }
}
