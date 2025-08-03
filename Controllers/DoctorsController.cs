using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.DTOs.Doctor;
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
        private readonly IMapper _mapper;

        public DoctorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors()
        {
            var doctors = await _context.Doctors.Include(d => d.Clinic).ToListAsync();
            var doctorDtos = _mapper.Map<List<DoctorDto>>(doctors);
            return Ok(doctorDtos);
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            return Ok(doctorDto);
        }

        // PUT: api/Doctors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, UpdateDoctorDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest();

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            _mapper.Map(updateDto, doctor);

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Doctors
        [HttpPost]
        public async Task<ActionResult<DoctorDto>> PostDoctor(CreateDoctorDto createDto)
        {
            var doctor = _mapper.Map<Doctor>(createDto);
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var doctorDto = _mapper.Map<DoctorDto>(doctor);

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctorDto);
        }

        // DELETE: api/Doctors/5
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
