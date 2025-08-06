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
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetPatients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients([FromQuery] PatientFilterRequest? patientFilterRequest, int page = 1)
        {
            var patiens = await _context.Patients.ToListAsync();
            //filter the name 
            if (patientFilterRequest is not null)
            {
                patiens = patiens.Where(e => e.FullName.Contains(patientFilterRequest.NamePationt)).ToList();
            }
            //filter the national
            if (patientFilterRequest.National is not null)
            {
                patiens = patiens.Where(e => e.NationalId == patientFilterRequest.National).ToList();
            }
            //filter the date of brith
            if (patientFilterRequest.dateOfBrith != null)
            {
                patiens = patiens.Where(e => e.DateOfBirth.Date == patientFilterRequest.dateOfBrith.Value.Date).ToList();
            }
            //filter the gender
            if (patientFilterRequest.gender is not null)
            {
                patiens = patiens.Where(e => e.Gender == patientFilterRequest.gender).ToList();
            }
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotallNumberOfPage = Math.Ceiling(patiens.Count() / 6.0),
                CurrentPage = page
            };
            //data
            var Retuens = new
            {
                namepatient = patientFilterRequest.NamePationt,
                gendetr = patientFilterRequest.gender,
                national = patientFilterRequest.National,
                dateofbrith = patientFilterRequest.dateOfBrith,
                patiens = patiens.Skip((page - 1) * 6).Take(6).ToList()
            };
            return Ok(new
            {
                pagination = Pagination,
                retuns = Retuens

            });

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            return patient;
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest();

            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
