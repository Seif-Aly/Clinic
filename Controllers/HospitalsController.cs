using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HospitalsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetHospitals")]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitals([FromQuery] HospitaliFilterRequest? hospitaliFilterRequest, int page = 1)
        {
            var hospitals = await _context.Hospitals.Include(e => e.Clinics).ToListAsync();
            //filter the name hospital
            if (!string.IsNullOrEmpty(hospitaliFilterRequest.NameHospital))
            {
                hospitals = hospitals.Where(e => e.Name.Contains(hospitaliFilterRequest.NameHospital)).ToList();
            }
            //filter the Address
            if (!string.IsNullOrEmpty(hospitaliFilterRequest.Address))
            {
                hospitals = hospitals.Where(e => e.Address == hospitaliFilterRequest.Address).ToList();
            }
            //pagination
            if (page < 0)
            {
                page = 1;
            }
            var Pagination = new
            {
                TotallNumberOfPage = Math.Ceiling(hospitals.Count() / 6.0),
                Currentpage = page
            };
            var Returns = new
            {
                namehospital = hospitaliFilterRequest.NameHospital,
                address = hospitaliFilterRequest.Address,
                hospitals = hospitals.Skip((page - 1) * 6).Take(6).ToList()

            };
            return Ok(new
            {
                pagination = Pagination,
                returns = Returns
            });
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Hospital>> GetHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);

            if (hospital == null)
            {
                return NotFound();
            }

            return hospital;
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospital(int id, Hospital hospital)
        {
            if (id != hospital.Id)
            {
                return BadRequest();
            }

            _context.Entry(hospital).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HospitalExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<Hospital>> PostHospital(Hospital hospital)
        {
            _context.Hospitals.Add(hospital);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHospital", new { id = hospital.Id }, hospital);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospital(int id)
        {
            var hospital = await _context.Hospitals.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }

            _context.Hospitals.Remove(hospital);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitals.Any(e => e.Id == id);
        }
    }
}
