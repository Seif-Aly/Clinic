using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Services.Interfaces;
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
            var result = await _doctorService.GetDoctorsAsync(doctorFilterRequest, page);
            return result != null ? Ok(result) : NotFound(new { message = "No doctors found matching the criteria." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            return doctor != null ? Ok(doctor) : NotFound(new { message = "No Doctor found with that ID." });
        }

        [HttpPost("PostDoctor")]
        public async Task<IActionResult> PostDoctor([FromForm] CreateDoctorDto createDoctorDto)
        {
            var result = await _doctorService.CreateDoctorAsync(createDoctorDto);
            if (!result.Success)
                return StatusCode(500, new { error = result.Message });

            return Ok(new { message = "Doctor added successfully" });
        }

        [HttpPut("PutDoctor/{id}")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] UpdateDoctorDto updateDoctorDto)
        {
            var result = await _doctorService.UpdateDoctorAsync(id, updateDoctorDto);
            if (!result.Success)
                return StatusCode(500, new { error = result.Message });

            return Ok(new { message = "Doctor updated successfully" });
        }

        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result.Success)
                return StatusCode(500, new { error = result.Message });

            return Ok(new { message = "Doctor deleted successfully" });
        }
    }
}
