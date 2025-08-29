using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Microsoft.EntityFrameworkCore;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly AppDbContext _context;

    public PrescriptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Prescription> prescriptions, int totalPages)> GetPrescriptionsAsync(PrescriptionFilterRequest? filter, int page, string? role, int? doctorId, int? patientId)
    {
        IQueryable<Prescription> query = _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient);

        if (role == "Patient" && patientId.HasValue)
            query = query.Where(p => p.PatientId == patientId.Value);
        else if (role == "Doctor" && doctorId.HasValue)
            query = query.Where(p => p.DoctorId == doctorId.Value);

        if (filter?.DoctorId is not null)
            query = query.Where(e => e.DoctorId == filter.DoctorId);
        if (!string.IsNullOrEmpty(filter?.NameDoctor))
            query = query.Where(e => e.Doctor.FullName.Contains(filter.NameDoctor));
        if (filter?.PationtId is not null)
            query = query.Where(e => e.PatientId == filter.PationtId);
        if (filter?.AppointmantId is not null)
            query = query.Where(e => e.AppointmentId == filter.AppointmantId);
        if (filter?.DateIssued != null)
            query = query.Where(e => e.DateIssued.Date == filter.DateIssued.Value.Date);

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / 6.0);

        var list = await query.OrderByDescending(p => p.DateIssued)
                              .Skip((page - 1) * 6)
                              .Take(6)
                              .ToListAsync();

        return (list, totalPages);
    }

    public async Task<Prescription?> GetPrescriptionByIdAsync(int id)
    {
        return await _context.Prescriptions
            .Include(p => p.PrescriptionItems)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointment)
                .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddPrescriptionAsync(Prescription prescription)
    {
        await _context.Prescriptions.AddAsync(prescription);
    }

    public async Task UpdatePrescriptionAsync(Prescription prescription)
    {
        _context.Prescriptions.Update(prescription);
    }

    public async Task DeletePrescriptionAsync(Prescription prescription)
    {
        _context.Prescriptions.Remove(prescription);
    }

    public async Task<bool> PrescriptionExistsAsync(int id)
    {
        return await _context.Prescriptions.AnyAsync(p => p.Id == id);
    }
}

