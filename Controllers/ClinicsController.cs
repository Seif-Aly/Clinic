using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ClinicsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetClinics")]
        public async Task<ActionResult<IEnumerable<Clinic>>> GetClinics([FromQuery] ClinicFilterRequest? clinicFilterRequest, int page = 1)
        {
            var clinic = await _context.Clinics.Include(c => c.Hospital).ToListAsync();
            //filter the name clinic
            if (clinicFilterRequest.NameClinic is not null)
            {
                clinic = clinic.Where(e => e.Name.Contains(clinicFilterRequest.NameClinic)).ToList();
            }
            //filter the name hospital
            if (clinicFilterRequest.NmaeHospitale is not null)
            {
                clinic = clinic.Where(e => e.Hospital.Name.Contains(clinicFilterRequest.NmaeHospitale)).ToList();
            }
            //filter the specialztion
            if (clinicFilterRequest.specialization is not null)
            {
                clinic = clinic.Where(e => e.Specialization == clinicFilterRequest.specialization).ToList();
            }
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotallNumberOfPage = Math.Ceiling(clinic.Count() / 6.0),
                CurrentPage = page
            };
            var Returns = new
            {
                nameclinic = clinicFilterRequest.NameClinic,
                namehospital = clinicFilterRequest.NmaeHospitale,
                spescialization = clinicFilterRequest.specialization,
                clinic = clinic.Skip((page - 1) * 6).Take(6).ToList()
            };
            return Ok(new
            {
                pagination = Pagination,
                returns = Returns
            });

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Clinic>> GetClinic(int id)
        {
            var clinic = await _context.Clinics.Include(c => c.Hospital).FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound();

            return clinic;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, Clinic clinic)
        {
            if (id != clinic.Id)
                return BadRequest();

            _context.Entry(clinic).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClinicExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Clinic>> PostClinic(Clinic clinic)
        {
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinic", new { id = clinic.Id }, clinic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }
    }
}
