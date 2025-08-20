using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicsController : ControllerBase
    {
        private readonly IClinicService _clinicService;

        public ClinicsController(IClinicService clinicService)
        {
            _clinicService = clinicService;
        }

        [HttpGet("GetClinics")]
        public async Task<IActionResult> GetClinics([FromQuery] ClinicFilterRequest? filter, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var (clinics, totalCount) = await _clinicService.GetClinicsAsync(filter, page);
                if (clinics == null || clinics.Count == 0)
                    return NotFound(new { message = "No clinics found matching the criteria." });

                var pagination = new
                {
                    TotalNumberOfPages = Math.Ceiling(totalCount / 6.0),
                    CurrentPage = page
                };

                var returns = new
                {
                    nameclinic = filter?.NameClinic,
                    namehospital = filter?.NmaeHospitale,
                    spescialization = filter?.specialization,
                    clinic = clinics
                };

                return Ok(new { pagination, returns });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching clinics.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClinic(int id)
        {
            try
            {
                var clinicDto = await _clinicService.GetClinicByIdAsync(id);
                if (clinicDto == null)
                    return NotFound(new { message = "No clinic found matching this ID." });

                return Ok(clinicDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while fetching the clinic.", details = ex.Message });
            }
        }

        [HttpPost("PostClinic")]
        public async Task<IActionResult> PostClinic([FromForm] CreateClinicDto dto)
        {
            try
            {
                var result = await _clinicService.AddClinicAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while adding the clinic.", details = ex.Message });
            }
        }

        [HttpPut("PutClinic/{id}")]
        public async Task<IActionResult> PutClinic(int id, [FromForm] UpdateClinicDto dto)
        {
            try
            {
                var result = await _clinicService.UpdateClinicAsync(id, dto);
                if (result == "Clinic not found.")
                    return NotFound(new { message = result });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the clinic.", details = ex.Message });
            }
        }

        [HttpDelete("DeleteClinic/{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            try
            {
                var result = await _clinicService.DeleteClinicAsync(id);
                if (result == "Clinic not found.")
                    return NotFound(new { message = result });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the clinic.", details = ex.Message });
            }
        }
    }
}
