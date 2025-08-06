using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic_Complex_Management_System1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        private const string RoleAdmin = "Admin";
        private const string RoleDoctor = "Doctor";
        private const string RolePatient = "Patient";

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        private string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);
        private int? CurrentDoctorId => int.TryParse(User.FindFirstValue("doctorId"), out var id) ? id : (int?)null;
        private int? CurrentPatientId => int.TryParse(User.FindFirstValue("patientId"), out var id) ? id : (int?)null;

        [HttpGet("GetAppointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments([FromQuery] AppointmantFilterReqest? appointmantFilterRequest, int page = 1)
        {
            IQueryable<Appointment> query = _context.Appointments
                .Include(e => e.Doctor)
                .Include(e => e.Patient);

            if (CurrentRole == RolePatient && CurrentPatientId is not null)
            {
                query = query.Where(a => a.PatientId == CurrentPatientId.Value);
            }
            else if (CurrentRole == RoleDoctor && CurrentDoctorId is not null)
            {
                query = query.Where(a => a.DoctorId == CurrentDoctorId.Value);
            }

            if (appointmantFilterRequest?.NameDoctor is not null)
                query = query.Where(e => e.Doctor.FullName.Contains(appointmantFilterRequest.NameDoctor));
            if (appointmantFilterRequest?.Specialization is not null)
                query = query.Where(e => e.Doctor.Specialization.Contains(appointmantFilterRequest.Specialization));
            if (appointmantFilterRequest?.date != null)
                query = query.Where(e => e.AppointmentDateTime.Date == appointmantFilterRequest.date.Value.Date);
            if (appointmantFilterRequest?.stutas != null)
                query = query.Where(e => e.stutas == appointmantFilterRequest.stutas);

            if (page < 1) page = 1;

            var total = await query.CountAsync();
            var list = await query
                .OrderByDescending(a => a.AppointmentDateTime)
                .Skip((page - 1) * 6)
                .Take(6)
                .ToListAsync();

            return Ok(new
            {
                Pagination = new { TotalNumberOfpage = Math.Ceiling(total / 6.0), CurrentPage = page },
                Returne = new
                {
                    nameDoctor = appointmantFilterRequest?.NameDoctor,
                    specialization = appointmantFilterRequest?.Specialization,
                    Stutas = appointmantFilterRequest?.stutas,
                    dateTime = appointmantFilterRequest?.date,
                    appointmant = list
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // Access check
            if (CurrentRole == RolePatient && CurrentPatientId != appointment.PatientId)
                return Forbid();
            if (CurrentRole == RoleDoctor && CurrentDoctorId != appointment.DoctorId)
                return Forbid();

            return appointment;
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleAdmin},{RoleDoctor}")]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            if (CurrentRole == RoleDoctor)
            {
                if (CurrentDoctorId is null || appointment.DoctorId != CurrentDoctorId.Value)
                    return Forbid();
            }

            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);

            if (!patientExists || !doctorExists)
                return BadRequest("Doctor or Patient not found.");

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return BadRequest();

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Appointments.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
