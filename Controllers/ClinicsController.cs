using AutoMapper;
using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Models;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutClinic(int id, UpdateClinicDto clinic)
        {
            if (id != clinic.Id)
                return BadRequest();
            var existingClinic = await _context.Clinics.FindAsync(id);
            if (existingClinic == null)
                return NotFound();
            existingClinic.Name = clinic.Name;
            existingClinic.Specialization = clinic.Specialization;
            existingClinic.HospitalId = clinic.HospitalId;
            _context.Entry(existingClinic).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (Exception ex)
            {
                if (!ClinicExists(id)) return NotFound(new { message = "No clinics found matching this ID." });

                else
                    return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });

            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ClinicDto>> PostClinic(CreateClinicDto clinic)
        {
            if (clinic == null)
                return BadRequest("Clinic data is required.");
            var clinicEntity = _mapper.Map<Clinic>(clinic);
            _context.Clinics.Add(clinicEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClinic", new { id = clinicEntity.Id }, clinic);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return NotFound(new { message = "No clinics found matching this ID." });

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClinicExists(int id)
        {
            return _context.Clinics.Any(e => e.Id == id);
        }
    }
}
