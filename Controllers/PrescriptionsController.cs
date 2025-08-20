using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _service;

    private string? CurrentRole => User.FindFirstValue(ClaimTypes.Role);
    private int? CurrentDoctorId => int.TryParse(User.FindFirstValue("doctorId"), out var id) ? id : (int?)null;
    private int? CurrentPatientId => int.TryParse(User.FindFirstValue("patientId"), out var id) ? id : (int?)null;

    public PrescriptionsController(IPrescriptionService service)
    {
        _service = service;
    }

    [HttpGet("GetAllPrescriptions")]
    public async Task<IActionResult> GetAllPrescriptions([FromQuery] PrescriptionFilterRequest? filterRequest, int page = 1)
    {
        if (page < 1) page = 1;

        var (prescriptions, totalPages) = await _service.GetPrescriptionsAsync(filterRequest, page, CurrentRole, CurrentDoctorId, CurrentPatientId);

        return Ok(new
        {
            Pagination = new { TotallNumberOfPage = totalPages, currentpage = page },
            Data = prescriptions
        });
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetPrescriptionDetails(int id)
    {
        var prescription = await _service.GetPrescriptionByIdAsync(id, CurrentRole, CurrentDoctorId, CurrentPatientId);
        if (prescription == null) return Forbid();

        return Ok(prescription);
    }

    [HttpPost("with-items")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> CreatePrescriptionWithItems([FromBody] PrescriptionWithItemsVM model)
    {
        var success = await _service.CreatePrescriptionAsync(model, CurrentRole, CurrentDoctorId);
        if (!success) return BadRequest("Failed to create prescription.");

        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionWithItemsVM model)
    {
        var success = await _service.UpdatePrescriptionAsync(id, model, CurrentRole, CurrentDoctorId);
        if (!success) return BadRequest("Failed to update prescription.");

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<IActionResult> DeletePrescriptionWithItems(int id)
    {
        var success = await
