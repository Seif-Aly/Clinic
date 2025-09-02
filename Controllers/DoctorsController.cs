using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Services.Base;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;
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
            var doctors = createDoctorDto.Adapt<Doctor>();

            try
            {
                if (createDoctorDto.Image != null && createDoctorDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(createDoctorDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await createDoctorDto.Image.CopyToAsync(stream);
                    }
                    doctors.images = filename; // نخزن اسم الملف فقط
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while saving image.", details = ex.Message });
            }
            await _doctorService.AddDoctorAsync(doctors);
            return Ok(new { message = "Successfully added doctor" });

            
        }

        [HttpPut("PutDoctor/{id}")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] UpdateDoctorDto updateDoctorDto)
        {
           
            try
            {
                var doctorInDb = await _doctorService.GetDoctorByIdAsync(id);
                if (doctorInDb == null)
                    return NotFound(new { message = "No doctor found matching this ID." });

                doctorInDb.FullName = updateDoctorDto.FullName;
                doctorInDb.Email = updateDoctorDto.Email;
                doctorInDb.Phone = updateDoctorDto.Phone;
                doctorInDb.Specialization = updateDoctorDto.Specialization;
                doctorInDb.ClinicId = updateDoctorDto.ClinicId;


                // التعامل مع الصورة
                if (updateDoctorDto.Image != null && updateDoctorDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateDoctorDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await updateDoctorDto.Image.CopyToAsync(stream);
                    }

                    // حذف الصورة القديمة
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctorInDb.images);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    doctorInDb.images = filename; // نخزن اسم الملف مش المسار الكامل
                }
                else
                {
                    doctorInDb.images = doctorInDb.images;
                }

                await _doctorService.UpdateDoctorAsync(doctorInDb);

               return Ok(new { message = "Successfully updated hospital" });
              
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
        }

        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
           
                var doctor = await _doctorService.GetDoctorByIdAsync(id);
                if (doctor == null)
                    return NotFound(new { message = "No doctor found matching this ID." });
            try
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "doctor", doctor.images);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                

                 await _doctorService.DeleteDoctorAsync(id);

                 return Ok(new { message = "Successfully deleted doctor" });
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting doctor from database.", details = ex.Message });
            }
        }
    }
}
