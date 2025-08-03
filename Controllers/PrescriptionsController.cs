using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTOs.Prescription;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PrescriptionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPrescriptions()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .ToListAsync();

            var result = _mapper.Map<List<PrescriptionDto>>(prescriptions);
            return Ok(result);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            var result = _mapper.Map<PrescriptionDto>(prescription);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription([FromBody] CreatePrescriptionDto dto)
        {
            var prescription = _mapper.Map<Prescription>(dto);
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PrescriptionDto>(prescription);
            return CreatedAtAction(nameof(GetPrescriptionDetails), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto dto)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            _mapper.Map(dto, prescription);

            _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);
            prescription.PrescriptionItems = _mapper.Map<List<PrescriptionItem>>(dto.Items);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionWithItems(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetPrescriptionsByPatient(int patientId)
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();

            var result = _mapper.Map<List<PrescriptionDto>>(prescriptions);
            return Ok(result);
        }

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<IActionResult> GetPrescriptionsByDoctor(int doctorId)
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();

            var result = _mapper.Map<List<PrescriptionDto>>(prescriptions);
            return Ok(result);
        }
    }
}