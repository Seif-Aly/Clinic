using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetDoctors")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors([FromQuery] DoctorFilterRequest? doctorFilterRequest, int page = 1)
        {
            var doctor = _context.Doctors.
                Include(d => d.Clinic)
                .Include(e => e.Appointments)
                .ToList();
            //filter the name doctor
            if (doctorFilterRequest.NameDoctor is not null)
            {
                doctor = doctor.Where(e => e.FullName.Contains(doctorFilterRequest.NameDoctor)).ToList();
            }
            //filter the name clinic
            if (doctorFilterRequest.NameClinic is not null)
            {
                doctor = doctor.Where(e => e.Clinic.Name.Contains(doctorFilterRequest.NameClinic)).ToList();
            }
            //filter tne specialization
            if (doctorFilterRequest.Specialization is not null)
            {
                doctor = doctor.Where(e => e.Specialization.Contains(doctorFilterRequest.Specialization)).ToList();
            }
            //pagiantion
            if (page < 0)
            {
                page = 1;
            }

            var pagiantion = new
            {
                TotalNumperOfPage = Math.Ceiling(doctor.Count() / 6.0),
                currentPage = page,

            };

            var returN = new
            {
                namedoctor = doctorFilterRequest.NameDoctor,
                nameclinic = doctorFilterRequest.NameClinic,
                specailzation = doctorFilterRequest.Specialization,
                doctor = doctor.Skip((page - 1) * 6).Take(6).ToList()

            };
            return Ok(new
            {
                Pagaination = pagiantion,
                Return = returN
            });


        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            return doctor;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.Id)
                return BadRequest();

            _context.Entry(doctor).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctor", new { id = doctor.Id }, doctor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
