using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ClinicsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Clinics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClinicDto>>> GetClinics()
        {
            var clinics = await _context.Clinics.Include(c => c.Hospital).ToListAsync();
            var clinicDtos = _mapper.Map<List<ClinicDto>>(clinics);
            return Ok(clinicDtos);
        }

        // GET: api/Clinics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDto>> GetClinic(int id)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Hospital)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound();

            var clinicDto = _mapper.Map<ClinicDto>(clinic);
            return Ok(clinicDto);
        }

        // PUT: api/Clinics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, UpdateClinicDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest();

            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound();

            var hospitalExists = await _context.Hospitals.AnyAsync(h => h.Id == updateDto.HospitalId);
            if (!hospitalExists)
                return BadRequest("hospital is not exist");

            _mapper.Map(updateDto, clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Clinics
        [HttpPost]
        public async Task<ActionResult<ClinicDto>> PostClinic(CreateClinicDto createDto)
        {
            var hospitalExists = await _context.Hospitals.AnyAsync(h => h.Id == createDto.HospitalId);
            if (!hospitalExists)
                return BadRequest("hospital is not exist");

            var clinic = _mapper.Map<Clinic>(createDto);
            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            var clinicDto = _mapper.Map<ClinicDto>(clinic);
            return CreatedAtAction(nameof(GetClinic), new { id = clinic.Id }, clinicDto);
        }

        // DELETE: api/Clinics/5
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