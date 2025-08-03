using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PatientsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            var patients = await _context.Patients.ToListAsync();
            var patientDtos = _mapper.Map<List<PatientDto>>(patients);
            return Ok(patientDtos);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            var patientDto = _mapper.Map<PatientDto>(patient);
            return Ok(patientDto);
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<PatientDto>> PostPatient(CreatePatientDto createDto)
        {
            var patient = _mapper.Map<Patient>(createDto);
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            var patientDto = _mapper.Map<PatientDto>(patient);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patientDto);
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, UpdatePatientDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            _mapper.Map(updateDto, patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Patients/5
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

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}