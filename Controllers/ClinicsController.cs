using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ClinicsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ClinicsController(AppDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetClinics")]
        public async Task<ActionResult<List<ClinicDto>>> GetClinics([FromQuery] ClinicFilterRequest? clinicFilterRequest, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var query = _context.Clinics.Include(c => c.Hospital).AsQueryable();

                if (clinicFilterRequest?.NameClinic is not null)
                    query = query.Where(e => e.Name.Contains(clinicFilterRequest.NameClinic));

                if (clinicFilterRequest?.NmaeHospitale is not null)
                    query = query.Where(e => e.Hospital != null && e.Hospital.Name.Contains(clinicFilterRequest.NmaeHospitale));

                if (clinicFilterRequest?.specialization is not null)
                    query = query.Where(e => e.Specialization == clinicFilterRequest.specialization);

                var totalCount = await query.CountAsync();
                if (totalCount > 0)
                {
                    var clinics = await query.Skip((page - 1) * 6).Take(6).ToListAsync();
                    var clinicDtos = _mapper.Map<List<ClinicDto>>(clinics);
                    var pagination = new
                    {
                        TotallNumberOfPage = Math.Ceiling(totalCount / 6.0),
                        CurrentPage = page
                    };
                    var returns = new
                    {
                        nameclinic = clinicFilterRequest?.NameClinic,
                        namehospital = clinicFilterRequest?.NmaeHospitale,
                        spescialization = clinicFilterRequest?.specialization,
                        clinic = clinicDtos
                    };
                    return Ok(new { pagination, returns });
                }
                else
                {
                    return NotFound(new { message = "No clinics found matching the criteria." });
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ClinicDto>> GetClinic(int id)
        {
            try
            {
                var clinic = await _context.Clinics.Include(c => c.Hospital).FirstOrDefaultAsync(c => c.Id == id);
                if (clinic == null)
                    return NotFound(new { message = "No clinic found matching this ID." });
                var clinicDto = _mapper.Map<ClinicDto>(clinic);
                return Ok(clinicDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }


        }

        [HttpPut("PutClinic/{id}")]
        public async Task<IActionResult> PutClinic(int id, [FromForm] UpdateClinicDto updateClinicDto)
        {
            //if (id != clinic.Id)
            //    return BadRequest();

            //_context.Entry(clinic).State = EntityState.Modified;

            //try { await _context.SaveChangesAsync(); }
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ClinicExists(id)) return NotFound();
            //    else throw;
            //}
            var clinic = updateClinicDto.Adapt<Clinic>();
            var clinicOnDb = _context.Clinics.AsNoTracking().FirstOrDefault(e => e.Id == id);
            if (clinicOnDb is not null)
            {
                if (updateClinicDto.Image is not null && updateClinicDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateClinicDto.Image.FileName);
                    var pathname = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\clinic", filename);
                    try
                    {
                        // save image in wwwroot
                        using (var streem = System.IO.File.Create(pathname))
                        {
                            await updateClinicDto.Image.CopyToAsync(streem);
                        }
                        //save image in db
                        clinic.Image = pathname;
                        //delet old image 
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\clinic", clinicOnDb.Image);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, new { error = "An error occurred while updating Image", details = ex.Message });
                    }
                }
                else
                {
                    clinic.Image = clinicOnDb.Image;
                }
                try
                {
                    _context.Clinics.Update(clinic);
                    _context.SaveChanges();
                    return Ok("succseefull update clinic");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
                }

            }
            return BadRequest(new { message = "No clinics found matching this ID." });

        }
        [HttpPost("PostClinic")]
        public async Task<ActionResult<Clinic>> PostClinic([FromForm] CreateClinicDto createClinicDto)
        {
            var clinic = createClinicDto.Adapt<Clinic>();
            if (createClinicDto.Image is not null && createClinicDto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(createClinicDto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\clinic", filename);
                //save image in wwwroot
                try
                {
                    using (var streem = System.IO.File.Create(filepath))
                    {
                        createClinicDto.Image.CopyTo(streem);
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "An error occurred while Saving Image to Database.", details = ex.Message });
                }
                //save image in db
                clinic.Image = filepath;
                try
                {
                    _context.Clinics.Add(clinic);
                    _context.SaveChanges();
                    return Ok("successfull add clinic");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
                }

                // return CreatedAtAction("GetClinic", new { id = clinic.Id }, clinic);
            }
            return BadRequest(new { message = "Clinic data is required." });

        }

        [HttpDelete("DeleteClinic/{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
            {
                return NotFound(new { message = "No clinic found matching this ID." });
            }
            try
            {
                //delet image in db
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\clinic", clinic.Image);
                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = "An error occurred while Deleting Image", details = ex.Message });
            }
            try
            {
                _context.Clinics.Remove(clinic);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while Deleting Image from DataBase.", details = ex.Message });

            }
            return Ok("succseefull delete clinic");

        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }
    }
}
