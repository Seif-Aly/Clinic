using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Microsoft.EntityFrameworkCore;

public class PatientRepository : IPatientRepository
{
    private readonly AppDbContext _context;

    public PatientRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<Patient?> GetByIdAsync(int id)
    {
        return await _context.Patients.FindAsync(id);
    }
    public async Task<int> GetPatientIdByUserIdAsync(Guid userId)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        return patient?.Id ?? 0;
    }

    public async Task AddAsync(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Update(Patient patient)
    {
        _context.Patients.Update(patient);
        return (await _context.SaveChangesAsync()) > 0;
    }

    public async Task<bool> Remove(Patient patient)
    {
        _context.Patients.Remove(patient);
        return (await _context.SaveChangesAsync()) > 0;
    }


}
