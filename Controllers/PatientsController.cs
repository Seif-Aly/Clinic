using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic_Complex_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IMapper _mapper;

        public PatientsController(IPatientService patientService, IMapper mapper)
        {
            _patientService = patientService;
            _mapper = mapper;
        }

        [HttpGet("GetPatients")]
        public async Task<ActionResult> GetPatients([FromQuery] PatientFilterRequest? patientFilterRequest, int page = 1, int pageSize = 10)
        {
            var result = await _patientService.GetPatientsAsync(patientFilterRequest, page, pageSize);

            var pagination = new
            {
                TotallNumberOfPage = result.TotalPages,
                CurrentPage = page < 1 ? 1 : page
            };

            var returns = new
            {
                namepatient = patientFilterRequest?.NamePationt,
                gendetr = patientFilterRequest?.gender,
                national = patientFilterRequest?.National,
                dateofbrith = patientFilterRequest?.dateOfBrith,
                patients = result.Patients
            };

            return Ok(new
            {
                pagination,
                returns
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound(new { message = "Patient not found." });

            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            var created = await _patientService.CreatePatientAsync(patient);
            if (!created)
                return StatusCode(500, new { message = "Failed to save patient. Database error." });

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.Id)
                return BadRequest(new { message = "ID mismatch." });

            var updated = await _patientService.UpdatePatientAsync(patient);
            if (!updated)
                return StatusCode(500, new { message = "Failed to update patient. Database error." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var deleted = await _patientService.DeletePatientAsync(id);
            if (!deleted)
                return NotFound(new { message = "Patient not found or could not be deleted." });

            return NoContent();
        }
    }
}
