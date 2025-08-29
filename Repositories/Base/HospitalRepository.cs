using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class HospitalRepository : IHospitalRepository
{
    private readonly AppDbContext _context;

    public HospitalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hospital>> GetHospitalsAsync(HospitaliFilterRequest? filter, int page)
    {
        var query = _context.Hospitals.Include(h => h.Clinics).AsQueryable();

        if (!string.IsNullOrEmpty(filter?.NameHospital))
            query = query.Where(h => h.Name.Contains(filter.NameHospital));
        if (!string.IsNullOrEmpty(filter?.Address))
            query = query.Where(h => h.Address == filter.Address);

        if (page < 1) page = 1;

        return await query.Skip((page - 1) * 6).Take(6).ToListAsync();
    }

    public async Task<Hospital?> GetHospitalByIdAsync(int id)
    {
        return await _context.Hospitals.Include(h => h.Clinics).FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task AddHospitalAsync(Hospital hospital)
    {
        await _context.Hospitals.AddAsync(hospital);
    }

    public void UpdateHospital(Hospital hospital)
    {
        _context.Hospitals.Update(hospital);
    }

    public void DeleteHospital(Hospital hospital)
    {
        _context.Hospitals.Remove(hospital);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<int> GetTotalCountAsync(HospitaliFilterRequest? filter)
    {
        var query = _context.Hospitals.AsQueryable();

        if (!string.IsNullOrEmpty(filter?.NameHospital))
            query = query.Where(h => h.Name.Contains(filter.NameHospital));
        if (!string.IsNullOrEmpty(filter?.Address))
            query = query.Where(h => h.Address == filter.Address);

        return await query.CountAsync();
    }
}
