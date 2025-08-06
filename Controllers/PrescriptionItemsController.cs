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

        public PrescriptionItemsController(AppDbContext context)
        {
            _context = context;
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
            var prescriptionItem = await _context.PrescriptionItems.Include(p => p.Prescription)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescriptionItem == null)
                return NotFound();

            return prescriptionItem;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescriptionItem(int id, PrescriptionItem prescriptionItem)
        {
            if (id != prescriptionItem.Id)
                return BadRequest();

            _context.Entry(prescriptionItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.PrescriptionItems.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<PrescriptionItem>> PostPrescriptionItem(PrescriptionItem prescriptionItem)
        {
            _context.PrescriptionItems.Add(prescriptionItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPrescriptionItem), new { id = prescriptionItem.Id }, prescriptionItem);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionItem(int id)
        {
            var prescriptionItem = await _context.PrescriptionItems.FindAsync(id);
            if (prescriptionItem == null)
                return NotFound();

            _context.PrescriptionItems.Remove(prescriptionItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
