using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clinic_Complex_Management_System1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private const string RoleAdmin = "Admin";
        private const string RoleDoctor = "Doctor";
        private const string RolePatient = "Patient";

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        private string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);
        private int? CurrentDoctorId => int.TryParse(User.FindFirstValue("doctorId"), out var id) ? id : (int?)null;
        private int? CurrentPatientId => int.TryParse(User.FindFirstValue("patientId"), out var id) ? id : (int?)null;

        [HttpGet("GetAppointments")]
        public async Task<IActionResult> GetAppointments([FromQuery] AppointmantFilterReqest? appointmantFilterRequest, int page = 1)
        {
            try
            {
                var (appointments, totalPages) = await _appointmentService.GetAppointments(
                    appointmantFilterRequest,
                    CurrentRole!,
                    CurrentDoctorId,
                    CurrentPatientId,
                    page
                );

                if (!appointments.Any())
                    return BadRequest(new { message = "No clinics found matching your criteria ID." });

                return Ok(new
                {
                    Pagination = new { TotalNumberOfPage = totalPages, CurrentPage = page },
                    Returne = new
                    {
                        nameDoctor = appointmantFilterRequest?.NameDoctor,
                        specialization = appointmantFilterRequest?.Specialization,
                        Status = appointmantFilterRequest?.status,
                        dateTime = appointmantFilterRequest?.date,
                        appointmant = appointments
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while processing your request.",
                    details = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointment(id, CurrentRole!, CurrentDoctorId, CurrentPatientId);
                if (appointment == null)
                    return Forbid("You have no access to that appointment or it doesn't exist.");

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while getting your appointment.",
                    details = ex.Message
                });
            }
        }

        [HttpPost]
        [Authorize(Roles = $"{RoleAdmin},{RoleDoctor}")]
        public async Task<IActionResult> CreateAppointment(CreateAppointmentDto appointmentDto)
        {
            try
            {
                var result = await _appointmentService.CreateAppointment(appointmentDto, CurrentRole!, CurrentDoctorId);
                if (result == null)
                    return BadRequest(new { message = "Doctor or Patient not found, or you have no access." });

                return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating your appointment.",
                    details = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> UpdateAppointment(int id, UpdateAppointmentDto appointmentDto)
        {
            if (id != appointmentDto.Id)
                return BadRequest(new { message = "Appointment ID mismatch" });

            var success = await _appointmentService.UpdateAppointment(appointmentDto);
            if (!success)
                return NotFound(new { message = "Appointment not found." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleAdmin)]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var success = await _appointmentService.DeleteAppointment(id);
            if (!success)
                return NotFound(new { message = "Appointment not found." });

            return NoContent();
        }
    }
}
