using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet("GetDoctors")]
        public async Task<IActionResult> GetDoctors([FromQuery] DoctorFilterRequest? doctorFilterRequest, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var result = await _doctorService.GetDoctorsAsync(doctorFilterRequest, page);
                if (result == null || result.TotalCount == 0)
                    return NotFound(new { message = "No doctors found matching the criteria." });

                var pagination = new
                {
                    TotalNumberOfPages = Math.Ceiling(result.TotalCount / 20.0),
                    CurrentPage = page
                };

                var returns = new
                {
                    nameclinic = doctorFilterRequest?.NameClinic,
                    namedoctor = doctorFilterRequest?.NameDoctor,
                    spescialization = doctorFilterRequest?.Specialization,
                    doctor = result.Doctors
                };

                // return Ok(new { pagination, returns });
                return Ok(new { doctors = result.Doctors, pagination });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching clinics.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            return doctor != null ? Ok(doctor) : NotFound(new { message = "No Doctor found with that ID." });
        }

        [HttpPost("PostDoctor")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostDoctor([FromForm] CreateDoctorDto createDoctorDto)
        {
            await _doctorService.AddDoctorAsync(createDoctorDto);
            return Ok(new { message = "Successfully added doctor" });
        }

        [HttpPut("PutDoctor/{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] UpdateDoctorDto updateDoctorDto)
        {
            var result = await _doctorService.UpdateDoctorAsync(updateDoctorDto, id);

            return result ? Ok(new { message = "Successfully updated hospital" }) : StatusCode(500, new { error = "An error occurred while processing your request." });
        }

        [HttpDelete("DeleteDoctor/{id}")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = "No doctor found matching this ID." });

            try
            {
                // Declare the variable before the if block
                string? oldFilePath = null;

                if (!string.IsNullOrEmpty(doctor.Image))
                {
                    oldFilePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images",
                        "doctor",
                        doctor.Image
                    );

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                await _doctorService.DeleteDoctorAsync(id);

                return Ok(new { message = "Successfully deleted doctor" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while deleting doctor from database.",
                    details = ex.Message
                });
            }
        }

    }
}
