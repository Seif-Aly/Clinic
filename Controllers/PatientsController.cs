using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Clinic_Complex_Management_System.Data;
using System.Security.Claims;
using Clinic_Complex_Management_System1.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System1.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var guid) ? guid : throw new Exception("Invalid User ID in token.");
        }
    }
}
namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;

        private readonly AppDbContext _context;

        public PatientsController(IPatientService patientService, IMapper mapper, AppDbContext context)
        {
            _patientService = patientService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("GetPatients")]
        public async Task<ActionResult> GetPatients(
            [FromQuery] PatientFilterRequest? patientFilterRequest,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var result = await _patientService.GetPatientsAsync(patientFilterRequest, page, pageSize);

                var pagination = new
                {
                    TotalNumberOfPages = result.TotalPages,
                    CurrentPage = page < 1 ? 1 : page
                };

                var returns = new
                {
                    namepatient = patientFilterRequest?.NamePatient,
                    gender = patientFilterRequest?.gender,
                    national = patientFilterRequest?.National,
                    dateOfBirth = patientFilterRequest?.dateOfBrith,
                    patients = result.Patients
                };

                return Ok(new { pagination, returns });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching patients.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(patient);
        }

        [HttpPost("PostPatient")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostPatient([FromBody] CreatePatientDto patientDto)
        {
            try
            {
                var created = await _patientService.CreatePatientAsync(patientDto);
                if (created == null)
                    return StatusCode(500, new { message = "Failed to save patient. Database error." });

                return Ok(new { message = "Successfully added patient" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the patient.", details = ex.Message });
            }
        }

        [HttpPut("PutPatient/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutPatient(int id, [FromBody] UpdatePatientDto patientDto)
        {
            try
            {
                var updated = await _patientService.UpdatePatientAsync(patientDto, id);
                if (!updated)
                    return StatusCode(500, new { message = "Failed to update patient. Database error." });

                return Ok(new { message = "Successfully updated patient" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating patient.", details = ex.Message });
            }
        }

        [HttpDelete("DeletePatient/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var deleted = await _patientService.DeletePatientAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Patient not found or could not be deleted." });

                return Ok(new { message = "Successfully deleted patient" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting patient.", details = ex.Message });
            }
        }

        [HttpGet("GetMyProfile")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId(); 
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient == null) return NotFound(new { message = "No profile found." });

            return Ok(patient);
        }


    }
}

