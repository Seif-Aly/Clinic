using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;
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
                        hospitals = hospitalDtos.Skip((page - 1) * 6).Take(6).ToList()
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



        [HttpGet("{id}")]
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


        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospital(int id, UpdateHospitalDto hospital)
        {
            if (id != hospital.Id)
            {
                return BadRequest();
            }
            var existingHospital = await _context.Hospitals.FindAsync(id);
            if (existingHospital == null)
            {
                return NotFound(new { message = "No hospitals found matching this ID." });
            }
            existingHospital.Name = hospital.Name;
            existingHospital.Address = hospital.Address;
            existingHospital.Phone = hospital.Phone;
            _context.Entry(existingHospital).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!HospitalExists(id))
                {
                    return NotFound(new { message = "No hospitals found matching this ID." });
                }
                else
                {
                    return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<HospitalDto>> PostHospital(CreateHospitalDto hospital)
        {
            if (string.IsNullOrWhiteSpace(hospital.Name) || string.IsNullOrWhiteSpace(hospital.Address) || string.IsNullOrWhiteSpace(hospital.Phone))
            {
                return BadRequest("Name, Address, and Phone are required.");
            }
            var hospitalEntity = _mapper.Map<Hospital>(hospital);
            _context.Hospitals.Add(hospitalEntity);
            await _context.SaveChangesAsync();
            var hospitalDto = _mapper.Map<HospitalDto>(hospitalEntity);
            return CreatedAtAction("GetHospital", new { id = hospitalDto.Id }, hospitalDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound(new { message = "No hospitals found matching this ID." });
            }
            try
            {
                _context.Hospitals.Remove(hospital);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }


        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}
