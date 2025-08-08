using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PrescriptionItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }



        [HttpGet("GetPrescriptionItems")]
        public async Task<ActionResult<IEnumerable<PrescriptionItem>>> GetPrescriptionItems([FromQuery] PrescriptionITemFilterRequest? prescriptionITemFilterRequest, int page = 1)
        {
            var prescriptionItem = await _context.PrescriptionItems.Include(p => p.Prescription).ToListAsync();
            // filter the name medical
            if (prescriptionITemFilterRequest.MedicalName is not null)
            {
                prescriptionItem = prescriptionItem.Where(e => e.MedicineName.Contains(prescriptionITemFilterRequest.MedicalName)).ToList();

            }
            //filter the presctiptionId
            if (prescriptionITemFilterRequest.PresctiptionId is not null)
            {
                prescriptionItem = prescriptionItem.Where(e => e.PrescriptionId == prescriptionITemFilterRequest.PresctiptionId).ToList();
            }
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotallNumberOfPage = Math.Ceiling(prescriptionItem.Count() / 6.0),
                CurrentPage = page
            };
            // data
            var Returns = new
            {
                namemedical = prescriptionITemFilterRequest.MedicalName,
                prescriptionId = prescriptionITemFilterRequest.PresctiptionId,
                prescriptionItem = prescriptionItem.Skip((page - 1) * 6).Take(6).ToList()
            };
            return Ok(new
            {
                pagination = Pagination,
                returns = Returns
            });

        }



        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionItem>> GetPrescriptionItem(int id)
        {
            var item = await _context.PrescriptionItems
                                     .Include(p => p.Prescription)
                                     .FirstOrDefaultAsync(p => p.Id == id);
            if (item == null)
                return NotFound(new { message = "Prescription item not found." });

            return Ok(item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescriptionItem(int id, PrescriptionItem item)
        {
            if (id != item.Id)
                return BadRequest(new { message = "ID mismatch." });

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.PrescriptionItems.AnyAsync(e => e.Id == id))
                    return NotFound(new { message = "Prescription item not found." });
                return StatusCode(409, new { message = "Concurrency conflict occurred while updating. Please retry." });
            }
        }



        [HttpPost]
        public async Task<ActionResult<PrescriptionItem>> PostPrescriptionItem(PrescriptionItem item)
        {
            try
            {
                _context.PrescriptionItems.Add(item);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPrescriptionItem), new { id = item.Id }, item);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Failed to save prescription item. Database error." });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionItem(int id)
        {
            var item = await _context.PrescriptionItems.FindAsync(id);
            if (item == null)
                return NotFound(new { message = "Prescription item not found." });

            try
            {
                _context.PrescriptionItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Failed to delete prescription item. Database error." });
            }
        }
    }
}
