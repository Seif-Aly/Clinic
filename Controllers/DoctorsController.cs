using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DoctorsController(AppDbContext context,
            IMapper mapper
            )
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("GetDoctors")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetDoctors([FromQuery] DoctorFilterRequest? doctorFilterRequest, int page = 1)
        {
            var query = _context.Doctors.
                Include(d => d.Clinic)
                .Include(e => e.Appointments)
                .AsQueryable();
            //filter the name doctor
            if (doctorFilterRequest.NameDoctor is not null)
            {
                query = query.Where(e => e.FullName.Contains(doctorFilterRequest.NameDoctor));
            }
            //filter the name clinic
            if (doctorFilterRequest.NameClinic is not null)
            {
                query = query.Where(e => e.Clinic.Name.Contains(doctorFilterRequest.NameClinic));
            }
            //filter tne specialization
            if (doctorFilterRequest.Specialization is not null)
            {
                query = query.Where(e => e.Specialization.Contains(doctorFilterRequest.Specialization));
            }
            //pagiantion
            if (page < 0)
            {
                page = 1;
            }
            var totalCount = await query.CountAsync();
            if (totalCount > 0)
            {
                var pagiantion = new
                {
                    TotalNumperOfPage = Math.Ceiling(totalCount / 6.0),
                    currentPage = page,

                };
                try
                {
                    var doctordto = _mapper.Map<List<DoctorDto>>(await query.Skip((page - 1) * 6).Take(6).ToListAsync());
                    var returN = new
                    {
                        namedoctor = doctorFilterRequest.NameDoctor,
                        nameclinic = doctorFilterRequest.NameClinic,
                        specailzation = doctorFilterRequest.Specialization,
                        doctor = doctordto

                    };
                    return Ok(new
                    {
                        Pagaination = pagiantion,
                        Return = returN
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
                }

            }
            else
                return NotFound(new { message = "No doctors found matching the criteria." });




        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == id);
                if (doctor == null)
                    return NotFound(new { message = " No Doctor found with that ID" });
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                return doctorDto;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
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
            try
            {
                var doctorInDb = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                var doctors = updateDoctorDto.Adapt<Doctor>();

                if (doctorInDb is not null)
                {
                    if (updateDoctorDto.Image is not null && updateDoctorDto.Image.Length > 0)
                    {
                        var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateDoctorDto.Image.FileName);
                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                        //save image in wwwroot
                        try
                        {
                            using (var streem = System.IO.File.Create(filepath))
                            {
                                await updateDoctorDto.Image.CopyToAsync(streem);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { error = "An error occurred while saving the image.", details = ex.Message });
                        }


                        // delet image old hospital in wwwroot
                        var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctorInDb.images);
                        try
                        {

                            if (System.IO.File.Exists(oldfilepath))
                            {
                                System.IO.File.Delete(oldfilepath);
                            }
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { error = "An error occurred while deleting the old image.", details = ex.Message });
                        }
                        //save  image in db
                        doctors.images = filepath;

                    }
                    else
                    {
                        doctors.images = doctorInDb.images;
                    }
                    try
                    {
                        _context.Doctors.Update(doctors);
                        _context.SaveChanges();
                        return Ok("successfull update doctors");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = "An error occurred while updating the doctor.", details = ex.Message });
                    }
                    //return CreatedAtAction("GetDoctor", new { id = doctors.Id }, doctors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
            return BadRequest(new { message = "No doctors found matching this ID." });

        }

        // [HttpPost("PostDoctor")]
        //  public async Task<ActionResult<DoctorDto>> PostDoctor([FromForm] CreateDoctorDto createDoctorDto)
        //{
        //  var doctor = createDoctorDto.Adapt<Doctor>();
        //if (createDoctorDto.Image is not null && createDoctorDto.Image.Length > 0)
        //{
        //  var filename = Guid.NewGuid().ToString() + Path.GetExtension(createDoctorDto.Image.FileName);
        //var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
        //save image in wwwroot
        //try
        // {
        //   using (var streem = System.IO.File.Create(filepath))
        // {
        //   await createDoctorDto.Image.CopyToAsync(streem);
        //}
        //}
        //catch (Exception ex)
        //{

        //  return StatusCode(500, new { error = "An error occurred while saving the image.", details = ex.Message });
        //}

        //save image in db
        // doctor.images = filepath;
        // try
        // {
        //await _context.Doctors.AddAsync(doctor);
        //await _context.SaveChangesAsync();
        //  return Ok(new { message = "succssefull add doctors" });
        //}
        //catch (Exception ex)
        //{
        //    return StatusCode(500, new { error = "An error occurred while saving the doctor.", details = ex.Message });
        //  }
        //return CreatedAtAction("GetDoctor", new { id = doctor.Id },doctor );
        //}
        //  return BadRequest("doctors data is required.");
        //}


        [HttpPost("PostDoctor")]
        public async Task<ActionResult<DoctorDto>> PostDoctor([FromForm] CreateDoctorDto createDoctorDto)
        {
            var doctor = createDoctorDto.Adapt<Doctor>();

            if (createDoctorDto.Image is not null && createDoctorDto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(createDoctorDto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "doctor", filename);

                try
                {
                    
                    var directoryPath = Path.GetDirectoryName(filepath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await createDoctorDto.Image.CopyToAsync(stream);
                    }

                   
                    doctor.images = filename;

                    await _context.Doctors.AddAsync(doctor);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = "Doctor added successfully" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        error = "Error occurred while saving the doctor.",
                        details = ex.Message,
                        inner = ex.InnerException?.Message,
                        stack = ex.StackTrace 
                    });
                }
            }
            return BadRequest("Doctor data is required.");
        }


        [HttpDelete("DeleteDoctor/{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { message = "No doctors found matching this ID." });
                }
                // delet old image in wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctor.images);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                //delete image in db
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
                return Ok(new { message = "succseefull delete doctors" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting doctor from Database.", details = ex.Message });
            }

        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
