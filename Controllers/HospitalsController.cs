using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public HospitalsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Hospitals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HospitalDto>>> GetHospitals()
        {
            var hospitals = await _context.Hospitals.ToListAsync();
            var hospitalDtos = _mapper.Map<List<HospitalDto>>(hospitals);
            return Ok(hospitalDtos);
        }

        // GET: api/Hospitals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HospitalDto>> GetHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
                return NotFound();

            var hospitalDto = _mapper.Map<HospitalDto>(hospital);
            return Ok(hospitalDto);
        }

        // POST: api/Hospitals
        [HttpPost]
        public async Task<ActionResult<HospitalDto>> PostHospital(CreateHospitalDto createDto)
        {
            var hospital = _mapper.Map<Hospital>(createDto);
            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();

            var hospitalDto = _mapper.Map<HospitalDto>(hospital);
            return CreatedAtAction(nameof(GetHospital), new { id = hospital.Id }, hospitalDto);
        }

        // PUT: api/Hospitals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospital(int id, UpdateHospitalDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest();

            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
                return NotFound();

            _mapper.Map(updateDto, hospital);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Hospitals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
                return NotFound();

            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}