using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.ViewModels;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Clinic_Complex_Management_System.DTos.Request;
using System.Security.Claims;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const string RoleAdmin = "Admin";
        private const string RoleDoctor = "Doctor";
        private const string RolePatient = "Patient";

        public PrescriptionsController(AppDbContext context)
        {
            _context = context;
        }

        private string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);
        private int? CurrentDoctorId => int.TryParse(User.FindFirstValue("doctorId"), out var id) ? id : (int?)null;
        private int? CurrentPatientId => int.TryParse(User.FindFirstValue("patientId"), out var id) ? id : (int?)null;

        [HttpGet("GetAllPrescriptions")]
        public async Task<IActionResult> GetAllPrescriptions([FromQuery] PrescriptionFilterRequest? prescriptionFilterRequest, int page = 1)
        {
            IQueryable<Prescription> query = _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a.Patient);

            if (CurrentRole == RolePatient && CurrentPatientId is not null)
            {
                query = query.Where(p => p.PatientId == CurrentPatientId.Value);
            }
            else if (CurrentRole == RoleDoctor && CurrentDoctorId is not null)
            {
                query = query.Where(p => p.DoctorId == CurrentDoctorId.Value);
            }

            if (prescriptionFilterRequest?.DoctorId is not null)
                query = query.Where(e => e.DoctorId == prescriptionFilterRequest.DoctorId);
            if (prescriptionFilterRequest?.NameDoctor is not null)
                query = query.Where(e => e.Doctor.FullName.Contains(prescriptionFilterRequest.NameDoctor));
            if (prescriptionFilterRequest?.PationtId is not null)
                query = query.Where(e => e.PatientId == prescriptionFilterRequest.PationtId);
            if (prescriptionFilterRequest?.AppointmantId is not null)
                query = query.Where(e => e.AppointmentId == prescriptionFilterRequest.AppointmantId);
            if (prescriptionFilterRequest?.DateIssued != null)
                query = query.Where(e => e.DateIssued.Date == prescriptionFilterRequest.DateIssued.Value.Date);

            if (page < 1) page = 1;

            var total = await query.CountAsync();
            var list = await query.OrderByDescending(p => p.DateIssued)
                                  .Skip((page - 1) * 6)
                                  .Take(6)
                                  .ToListAsync();

            return Ok(new
            {
                Pagination = new { TotallNumberOfPage = Math.Ceiling(total / 6.0), currentpage = page },
                Data = list
            });
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

            if (CurrentRole == RolePatient && CurrentPatientId != prescription.PatientId)
                return Forbid();
            if (CurrentRole == RoleDoctor && CurrentDoctorId != prescription.DoctorId)
                return Forbid();

            return Ok(prescription);
        }

        [HttpPost("with-items")]
        [Authorize(Roles = $"{RoleDoctor},{RoleAdmin}")]
        public async Task<IActionResult> CreatePrescriptionWithItems([FromBody] PrescriptionWithItemsVM model)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == model.AppointmentId);

            if (appointment == null)
                return BadRequest("Invalid Appointment");

            if (CurrentRole == RoleDoctor)
            {
                if (CurrentDoctorId is null || appointment.DoctorId != CurrentDoctorId.Value)
                    return Forbid();
            }

            var prescription = new Prescription
            {
                Diagnosis = model.Diagnosis,
                Notes = model.Notes,
                DateIssued = model.DateIssued,
                AppointmentId = model.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
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
        [Authorize(Roles = $"{RoleDoctor},{RoleAdmin}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionWithItemsVM model)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            if (CurrentRole == RoleDoctor)
            {
                if (CurrentDoctorId is null || prescription.Appointment.DoctorId != CurrentDoctorId.Value)
                    return Forbid();
            }

            prescription.Diagnosis = model.Diagnosis;
            prescription.Notes = model.Notes;
            prescription.DateIssued = model.DateIssued;

            if (prescription.AppointmentId != model.AppointmentId)
            {
                var appt = await _context.Appointments.FindAsync(model.AppointmentId);
                if (appt == null) return BadRequest("Invalid Appointment");

                if (CurrentRole == RoleDoctor && appt.DoctorId != CurrentDoctorId)
                    return Forbid();

                prescription.AppointmentId = appt.Id;
                prescription.DoctorId = appt.DoctorId;
                prescription.PatientId = appt.PatientId;
            }

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
        [Authorize(Roles = $"{RoleDoctor},{RoleAdmin}")]
        public async Task<IActionResult> DeletePrescriptionWithItems(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                return NotFound();

            if (CurrentRole == RoleDoctor)
            {
                if (CurrentDoctorId is null || prescription.Appointment.DoctorId != CurrentDoctorId.Value)
                    return Forbid();
            }

            _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);
            _context.Prescriptions.Remove(prescription);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByPatient(int patientId)
        {
            if (CurrentRole == RolePatient && CurrentPatientId != patientId)
                return Forbid();

            if (CurrentRole == RoleDoctor)
                return Forbid();

            return await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();
        }

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByDoctor(int doctorId)
        {
            if (CurrentRole == RoleDoctor && CurrentDoctorId != doctorId)
                return Forbid();

            if (CurrentRole == RolePatient)
                return Forbid();

            return await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId)
                .Include(p => p.PrescriptionItems)
                .ToListAsync();
        }
    }
}
