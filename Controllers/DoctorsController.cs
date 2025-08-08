using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetDoctors")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors([FromQuery] DoctorFilterRequest? doctorFilterRequest, int page = 1)
        {
            var doctor = _context.Doctors.
                Include(d => d.Clinic)
                .Include(e => e.Appointments)
                .ToList();
            //filter the name doctor
            if (doctorFilterRequest.NameDoctor is not null)
            {
                doctor = doctor.Where(e => e.FullName.Contains(doctorFilterRequest.NameDoctor)).ToList();
            }
            //filter the name clinic
            if (doctorFilterRequest.NameClinic is not null)
            {
                doctor = doctor.Where(e => e.Clinic.Name.Contains(doctorFilterRequest.NameClinic)).ToList();
            }
            //filter tne specialization
            if (doctorFilterRequest.Specialization is not null)
            {
                doctor = doctor.Where(e => e.Specialization.Contains(doctorFilterRequest.Specialization)).ToList();
            }
            //pagiantion
            if (page < 0)
            {
                page = 1;
            }

            var pagiantion = new
            {
                TotalNumperOfPage = Math.Ceiling(doctor.Count() / 6.0),
                currentPage = page,

            };

            var returN = new
            {
                namedoctor = doctorFilterRequest.NameDoctor,
                nameclinic = doctorFilterRequest.NameClinic,
                specailzation = doctorFilterRequest.Specialization,
                doctor = doctor.Skip((page - 1) * 6).Take(6).ToList()

            };
            return Ok(new
            {
                Pagaination = pagiantion,
                Return = returN
            });


        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            return doctor;
        }

        [HttpPut("PutDoctor/{id}")]
        public async Task<IActionResult> PutDoctor(int id, [FromForm] UpdateDoctorDto? updateDoctorDto)
        {
            //if (id != doctor.Id)
            //    return BadRequest();

            //_context.Entry(doctor).State = EntityState.Modified;

            //try { await _context.SaveChangesAsync(); }
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!DoctorExists(id)) return NotFound();
            //    else throw;
            //}
            var doctorInDb = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            var doctors = updateDoctorDto.Adapt<Doctor>();

            if (doctorInDb is not null)
            {
                if (updateDoctorDto.Image is not null && updateDoctorDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateDoctorDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                    //save image in wwwroot
                    using (var streem = System.IO.File.Create(filepath))
                    {
                        await updateDoctorDto.Image.CopyToAsync(streem);
                    }

                    // delet image old hospital in wwwroot
                    var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctorInDb.images);
                    if (System.IO.File.Exists(oldfilepath))
                    {
                        System.IO.File.Delete(oldfilepath);
                    }
                    //save  image in db
                    doctors.images = filepath;

                }
                else
                {
                    doctors.images = doctorInDb.images;
                }
                _context.Doctors.Update(doctors);
                _context.SaveChanges();
               return Ok("successfull update doctors");
               //return CreatedAtAction("GetDoctor", new { id = doctors.Id }, doctors);
            }

            return BadRequest(new { message = "No doctors found matching this ID." });

        }

        [HttpPost("PostDoctor")]
        public async Task<ActionResult<Doctor>> PostDoctor([FromForm] CreateDoctorDto createDoctorDto)
        {
            var doctor = createDoctorDto.Adapt<Doctor>();
            if (createDoctorDto.Image is not null && createDoctorDto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(createDoctorDto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                //save image in wwwroot
                using (var streem = System.IO.File.Create(filepath))
                {
                    await createDoctorDto.Image.CopyToAsync(streem);
                }
                //save image in db
                doctor.images = filepath;
               await _context.Doctors.AddAsync(doctor);
                await _context.SaveChangesAsync();
                return Ok("succssefull add doctors");
                //return CreatedAtAction("GetDoctor", new { id = doctor.Id },doctor );
            }
            return BadRequest("doctors data is required.");

        }
        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor =await _context.Doctors.FindAsync( id);
            if (doctor == null)
            {
                return NotFound(new { message = "No doctors found matching this ID." });
            }
            try
            {
                // delet old image in wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctor.images);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                //delete image in db
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return Ok("succseefull delete doctors");
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }

        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
