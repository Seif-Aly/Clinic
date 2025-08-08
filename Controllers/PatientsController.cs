using AutoMapper;
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

        private readonly IMapper _mapper;


        public PatientsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                return NotFound(new { message = "Patient not found." });

            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Failed to save patient. Database error." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest(new { message = "ID mismatch." });

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Failed to update patient. Database error." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Failed to delete patient. Database error." });
            }
        }
    }
}
