using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.ViewModels;
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

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPrescriptions()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .ToListAsync();

            return Ok(prescriptions);
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetPrescriptionDetails(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            return Ok(prescription);
        }

        
        [HttpPost("with-items")]
        public async Task<IActionResult> CreatePrescriptionWithItems([FromBody] PrescriptionWithItemsVM model)
        {
            var appointment = await _context.Appointments.FindAsync(model.AppointmentId);
            if (appointment == null)
                return BadRequest("Invalid Appointment");

            var prescription = new Prescription
            {
                Diagnosis = model.Diagnosis,
                Notes = model.Notes,
                DateIssued = model.DateIssued,
                AppointmentId = model.AppointmentId,
                DoctorId = model.DoctorId,
                PatientId = model.PatientId,
                PrescriptionItems = model.Items.Select(i => new PrescriptionItem
                {
                    MedicineName = i.MedicationName,
                    Dosage = i.Dosage,
                    Instructions = i.Instructions
                }).ToList()
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionWithItemsVM model)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            prescription.Diagnosis = model.Diagnosis;
            prescription.Notes = model.Notes;
            prescription.DateIssued = model.DateIssued;
            prescription.AppointmentId = model.AppointmentId;

            _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);

            prescription.PrescriptionItems = model.Items.Select(i => new PrescriptionItem
            {
                MedicineName = i.MedicationName,
                Dosage = i.Dosage,
                Instructions = i.Instructions
            }).ToList();

            await _context.SaveChangesAsync();

            return Ok(prescription);
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
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByPatient(int patientId)
        {
            return await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();
        }


        [HttpGet("by-doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByDoctor(int doctorId)
        {
            return await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();
        }
    }
}
