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
                var result = await _appointmentService.GetAppointments(
                    appointmantFilterRequest,
                    CurrentRole!,
                    CurrentDoctorId,
                    CurrentPatientId,
                    page
                );

                if (!result.Appointments.Any())
                    return BadRequest(new { message = "No Appoinments found matching your criteria ID." });

                return Ok(new
                {
                    Pagination = new { TotalNumberOfPage = result.TotalCount, CurrentPage = page },
                    Returne = new
                    {
                        nameDoctor = appointmantFilterRequest?.NameDoctor,
                        specialization = appointmantFilterRequest?.Specialization,
                        Status = appointmantFilterRequest?.status,
                        dateTime = appointmantFilterRequest?.date,
                        appointmant = result.Appointments
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
        public async Task<IActionResult> CreateAppointment([FromForm] CreateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.CreateAppointment(dto, CurrentRole!, CurrentDoctorId, CurrentPatientId);
                if (result == null)
                    return BadRequest(new { message = "Doctor or Patient not found, or no access." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error creating appointment", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromForm] UpdateAppointmentDto appointmentDto)
        {

            var success = await _appointmentService.UpdateAppointment(id, appointmentDto, CurrentRole!, CurrentDoctorId, CurrentPatientId);
            if (!success)
                return NotFound(new { message = "Appointment not found." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var success = await _appointmentService.DeleteAppointment(id);
            if (!success)
                return NotFound(new { message = "Appointment not found." });

            return NoContent();
        }
    }
}
