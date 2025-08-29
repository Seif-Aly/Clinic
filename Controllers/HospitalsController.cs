using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin")]
public class HospitalsController : ControllerBase
{
    private readonly IHospitalService _hospitalService;
    private readonly IMapper _mapper;
    private const int PageSize = 6;

    public HospitalsController(IHospitalService hospitalService, IMapper mapper)
    {
        _hospitalService = hospitalService;
        _mapper = mapper;
    }

    [HttpGet("GetHospitals")]
    public async Task<IActionResult> GetHospitals([FromQuery] HospitaliFilterRequest? filter, int page = 1)
    {
        try
        {
            var result = await _hospitalService.GetHospitalsAsync(filter, page);

            if (result == null || result.Hospitals == null || !result.Hospitals.Any())
                return NotFound(new { message = "No hospitals found matching the criteria." });


            var pagination = new
            {
                TotalNumberOfPages = Math.Ceiling(result.TotalCount / (double)PageSize),
                CurrentPage = page
            };
            var returns = new
            {
                namehospital = filter?.NameHospital,
                address = filter?.Address,
                hospitals = result.Hospitals
            };
            return Ok(new { pagination, returns });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
        }
    }

    [HttpGet("GetHospital/{id}")]
    public async Task<IActionResult> GetHospital(int id)
    {
        try
        {
            var hospital = await _hospitalService.GetHospitalByIdAsync(id);
            if (hospital == null)
                return NotFound(new { message = "No hospitals found matching this ID." });

            var hospitalDto = _mapper.Map<HospitalDto>(hospital);
            return Ok(hospitalDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
        }
    }

    [HttpPut("PutHospital/{id}")]
    public async Task<IActionResult> PutHospital(int id, [FromForm] UpdateHospitalDto updateHospitalDto)
    {
        try
        {
            var hospitalInDb = await _hospitalService.GetHospitalByIdAsync(id);
            if (hospitalInDb == null)
                return NotFound(new { message = "No hospitals found matching this ID." });

            hospitalInDb.Name = updateHospitalDto.Name;
            hospitalInDb.Address = updateHospitalDto.Address;
            hospitalInDb.Phone = updateHospitalDto.Phone;


            // التعامل مع الصورة
            if (updateHospitalDto.Image != null && updateHospitalDto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateHospitalDto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await updateHospitalDto.Image.CopyToAsync(stream);
                }

                // حذف الصورة القديمة
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", hospitalInDb.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                hospitalInDb.Image = filename; // نخزن اسم الملف مش المسار الكامل
            }
            else
            {
                hospitalInDb.Image = hospitalInDb.Image;
            }

            bool updated = await _hospitalService.UpdateHospitalAsync(hospitalInDb);

            if (updated)
                return Ok(new { message = "Successfully updated hospital" });
            else
                return StatusCode(500, new { error = "Failed to update hospital" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
        }
    }

    [HttpPost("PostHospital")]
    public async Task<IActionResult> PostHospital([FromForm] CreateHospitalDto createHospitalDto)
    {
        var hospital = createHospitalDto.Adapt<Hospital>();

        try
        {
            if (createHospitalDto.Image != null && createHospitalDto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(createHospitalDto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", filename);
                using (var stream = System.IO.File.Create(filepath))
                {
                    await createHospitalDto.Image.CopyToAsync(stream);
                }
                hospital.Image = filename; // نخزن اسم الملف فقط
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while saving image.", details = ex.Message });
        }

        try
        {
            bool created = await _hospitalService.AddHospitalAsync(hospital);
            if (created)
                return Ok(new { message = "Successfully added hospital" });
            else
                return StatusCode(500, new { error = "Failed to save hospital" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while saving hospital to database.", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHospital(int id)
    {
        try
        {
            var hospital = await _hospitalService.GetHospitalByIdAsync(id);
            if (hospital == null)
                return NotFound(new { message = "No hospitals found matching this ID." });

            if (!string.IsNullOrEmpty(hospital.Image))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Hospital", hospital.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            bool deleted = await _hospitalService.DeleteHospitalAsync(id);

            if (deleted)
                return Ok(new { message = "Successfully deleted hospital" });
            else
                return StatusCode(500, new { error = "Failed to delete hospital" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting hospital from database.", details = ex.Message });
        }
    }

}
