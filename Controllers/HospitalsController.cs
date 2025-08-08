using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public HospitalsController(AppDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("GetHospitals")]
        public async Task<ActionResult<IEnumerable<HospitalDto>>> GetHospitals([FromQuery] HospitaliFilterRequest? hospitaliFilterRequest, int page = 1)
        {
            try
            {
                var query = _context.Hospitals.Include(e => e.Clinics).AsQueryable();
                //filter the name hospital
                if (!string.IsNullOrEmpty(hospitaliFilterRequest.NameHospital))
                {
                    query = query.Where(e => e.Name.Contains(hospitaliFilterRequest.NameHospital));
                }
                //filter the Address
                if (!string.IsNullOrEmpty(hospitaliFilterRequest.Address))
                {
                    query = query.Where(e => e.Address == hospitaliFilterRequest.Address);
                }
                //pagination
                if (page < 0)
                {
                    page = 1;
                }
                var totalCount = await query.CountAsync();
                if (totalCount > 0)
                {
                    var hospitals = await query.Skip((page - 1) * 6).Take(6).ToListAsync();
                    var hospitalDtos = _mapper.Map<List<HospitalDto>>(hospitals);
                    var pagination = new
                    {
                        TotallNumberOfPage = Math.Ceiling(totalCount / 6.0),
                        CurrentPage = page
                    };
                    var returns = new
                    {
                        namehospital = hospitaliFilterRequest.NameHospital,
                        address = hospitaliFilterRequest.Address,
                        hospitals = hospitalDtos
                    };
                    return Ok(new { pagination, returns });
                }
                else
                {
                    return NotFound(new { message = "No hospitals found matching the criteria." });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
        }



        [HttpGet("GetHospital/{id}")]
        public async Task<ActionResult<HospitalDto>> GetHospital(int id)
        {
            try
            {
                var hospital = await _context.Hospitals.FindAsync(id);

                if (hospital == null)
                {
                    return NotFound(new { message = "No hospitals found matching this ID." });
                }
                var hospitalDto = _mapper.Map<HospitalDto>(hospital);
                return hospitalDto;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }
        }


        [HttpPut("PutHospital/{id}")]
        public async Task<IActionResult> PutHospital(int id, [FromForm] UpdateHospitalDto updateHospitalDto)
        {
            //if (id != hospital.Id)
            //{
            //    return BadRequest();
            //}
            //var existingHospital = await _context.Hospitals.FindAsync(id);
            //if (existingHospital == null)
            //{
            //    return NotFound(new { message = "No hospitals found matching this ID." });
            //}
            //existingHospital.Name = hospital.Name;
            //existingHospital.Address = hospital.Address;
            //existingHospital.Phone = hospital.Phone;
            //_context.Entry(existingHospital).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{
            //    if (!HospitalExists(id))
            //    {
            //        return NotFound(new { message = "No hospitals found matching this ID." });
            //    }
            //    else
            //    {
            //        return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            //    }
            //}

            //return NoContent();
            try
            {
                var hospitalindb = await _context.Hospitals.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                var hospitals = updateHospitalDto.Adapt<Hospital>();

                if (hospitalindb is not null)
                {
                    if (updateHospitalDto.Image is not null && updateHospitalDto.Image.Length > 0)
                    {
                        var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateHospitalDto.Image.FileName);
                        var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", filename);
                        //save image in wwwroot
                        using (var streem = System.IO.File.Create(filepath))
                        {
                            await updateHospitalDto.Image.CopyToAsync(streem);
                        }

                        // delet image old hospital in wwwroot
                        var oldfilepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", hospitalindb.Image);
                        if (System.IO.File.Exists(oldfilepath))
                        {
                            System.IO.File.Delete(oldfilepath);
                        }
                        //save  image in db
                        hospitals.Image = filepath;

                    }
                    else
                    {
                        hospitals.Image = hospitalindb.Image;
                    }
                    _context.Hospitals.Update(hospitals);
                    _context.SaveChanges();
                    return Ok(new { message = "successfull update hospital" });
                    //return CreatedAtAction("GetHospital", new { id = hospitals.Id}, hospitals);

                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }


            return BadRequest("hospital data is required.");
        }


        [HttpPost("PostHospital")]
        public async Task<ActionResult<Hospital>> PostHospital([FromForm] CreateHospitalDto createHospitalDto)
        {
            var hospital = createHospitalDto.Adapt<Hospital>();
            try
            {
                if (createHospitalDto.Image is not null && createHospitalDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(createHospitalDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", filename);
                    //save image in wwwroot
                    using (var streem = System.IO.File.Create(filepath))
                    {
                        await createHospitalDto.Image.CopyToAsync(streem);
                    }
                    //save image in db
                    hospital.Image = filepath;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while saving image.", details = ex.Message });
            }
            try
            {

                await _context.Hospitals.AddAsync(hospital);
                await _context.SaveChangesAsync();
                return Ok(new { message = "succssefull add hospital" });
                //return CreatedAtAction("GetHospital", new { id = hospital.Id }, hospital);
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
                var hospital = await _context.Hospitals.FindAsync(id);
                if (hospital == null)
                {
                    return NotFound(new { message = "No hospitals found matching this ID." });
                }
                // delet old image in wwwroot
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\Hospital", hospital.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                //delete image in db
                _context.Hospitals.Remove(hospital);
                await _context.SaveChangesAsync();
                return Ok(new { message = "succseefull delete hospital" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting hospital from database.", details = ex.Message });
            }


        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}
