using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinic_Complex_Management_System1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("GetAppointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments([FromQuery] AppointmantFilterReqest? appointmantFilterRequest, int page = 1)
        {


            var appointmant = await _context.Appointments
                .Include(e => e.Doctor)
                .Include(e => e.Patient)
                .ToListAsync();

            // Filter the name doctor
            if (appointmantFilterRequest.NameDoctor is not null)
            {
                appointmant = appointmant
                    .Where(e => e.Doctor.FullName.Contains(appointmantFilterRequest.NameDoctor))
                    .ToList();
            }
            // Filter the Specialization
            if (appointmantFilterRequest.Specialization is not null)
            {
                appointmant = appointmant
                    .Where(e => e.Doctor.Specialization.Contains(appointmantFilterRequest.Specialization))
                    .ToList();
            }
            // Filter the date time
            if (appointmantFilterRequest.date != null)
            {
                appointmant = appointmant
                    .Where(e => e.AppointmentDateTime.Date == appointmantFilterRequest.date.Value.Date)
                    .ToList();
            }
            //filter the status
            if (appointmantFilterRequest.stutas != null)
            {

                appointmant = appointmant
                    .Where(e => e.stutas == appointmantFilterRequest.stutas)
                    .ToList();

            }
            // pageiation
            if (page < 0)
                page = 1;
            var pagination = new
            {
                TotalNumberOfpage = Math.Ceiling(appointmant.Count() / 6.0),
                CurrentPage = page

            };
            var returne = new
            {
                nameDoctor = appointmantFilterRequest.NameDoctor,
                specialization = appointmantFilterRequest.Specialization,
                Stutas = appointmantFilterRequest.stutas,
                dateTime = appointmantFilterRequest.date,
                appointmant = appointmant.Skip((page - 1) * 6).Take(6).ToList()

            };

            return Ok(new
            {
                Pagination = pagination,
                Returne = returne

            });
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return appointment;
        }

        
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
           
            var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
            var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId);

            if (!patientExists || !doctorExists)
                return BadRequest("الطبيب أو المريض غير موجود");

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return BadRequest();

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
