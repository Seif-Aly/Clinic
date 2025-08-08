using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;
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
        private readonly IMapper _mapper;
        private const string RoleAdmin = "Admin";
        private const string RoleDoctor = "Doctor";
        private const string RolePatient = "Patient";

        public AppointmentsController(AppDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);
        private int? CurrentDoctorId => int.TryParse(User.FindFirstValue("doctorId"), out var id) ? id : (int?)null;
        private int? CurrentPatientId => int.TryParse(User.FindFirstValue("patientId"), out var id) ? id : (int?)null;

        [HttpGet("GetAppointments")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments([FromQuery] AppointmantFilterReqest? appointmantFilterRequest, int page = 1)
        {
            try
            {
                var query = _context.Appointments
                .Include(e => e.Doctor)
                .Include(e => e.Patient).AsQueryable();

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
                if (appointmantFilterRequest?.status != null)
                    query = query.Where(e => e.Status == appointmantFilterRequest.status);

                if (page < 1) page = 1;
                var total = await query.CountAsync();
                if (total > 0)
                {
                    var list = await query
                    .OrderByDescending(a => a.AppointmentDateTime)
                    .Skip((page - 1) * 6)
                    .Take(6)
                    .ToListAsync();
                    var appointmentDto = _mapper.Map<List<AppointmentDto>>(list);
                    return Ok(new
                    {
                        Pagination = new { TotalNumberOfpage = Math.Ceiling(total / 6.0), CurrentPage = page },
                        Returne = new
                        {
                            nameDoctor = appointmantFilterRequest?.NameDoctor,
                            specialization = appointmantFilterRequest?.Specialization,
                            Status = appointmantFilterRequest?.status,
                            dateTime = appointmantFilterRequest?.date,
                            appointmant = appointmentDto
                        }
                    });
                }
                else
                    return BadRequest(new { message = "No clinics found matching your criteria ID." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }



        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments
               .Include(a => a.Patient)
               .Include(a => a.Doctor)
               .FirstOrDefaultAsync(a => a.Id == id);
                if (appointment == null)
                    return NotFound(new { message = "No Appointment Found" });

                // Access check
                if (CurrentRole == RolePatient && CurrentPatientId != appointment.PatientId)
                    return Forbid("You have no access to that Apppointment");
                if (CurrentRole == RoleDoctor && CurrentDoctorId != appointment.DoctorId)
                    return Forbid("You have no access to that Apppointment");
                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
                return appointmentDto;
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = "An error occurred while getting your appointment.", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleAdmin},{RoleDoctor}")]
        public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto appointment)
        {
            if (CurrentRole == RoleDoctor)
            {
                if (CurrentDoctorId is null || appointment.DoctorId != CurrentDoctorId.Value)
                    return Forbid("You have no access to that Apppointment");
            }
            try
            {

                var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);

                if (!patientExists || !doctorExists)
                    return BadRequest(new { message = "Doctor or Patient not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while checking doctor or patient.", details = ex.Message });
            }
            try
            {
                var appointmentEntity = _mapper.Map<Appointment>(appointment);
                _context.Appointments.Add(appointmentEntity);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetAppointment), new { id = appointmentEntity.Id }, appointment);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating your appointment.", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDto appointmentDto)
        {
            if (id != appointmentDto.Id)
                return BadRequest(new { message = "No Appointment found with that Id" });
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound(new { message = "No Appointment found with that Id" });
            appointment.AppointmentDateTime = appointmentDto.AppointmentDateTime;
            appointment.Status = appointmentDto.Status;
            appointment.PatientId = appointmentDto.PatientId;
            appointment.DoctorId = appointmentDto.DoctorId;
            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!_context.Appointments.Any(e => e.Id == id))
                    return NotFound(new { message = "No Appointment found with that Id" });
                return StatusCode(500, new { error = "An error occurred while updating your appointment.", details = ex.Message });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                    return NotFound(new { message = "No Appointment found with that Id" });
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting your appointment.", details = ex.Message });
            }

            return NoContent();
        }
    }
}
