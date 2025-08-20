using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System1.Repositories.Base
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly AppDbContext _context;

        public ClinicRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Clinic>> GetClinicsAsync(string? nameClinic, string? nameHospital, string? specialization, int page, int pageSize)
        {
            var query = _context.Clinics.Include(c => c.Hospital).AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameClinic))
                query = query.Where(e => e.Name!.Contains(nameClinic));

            if (!string.IsNullOrWhiteSpace(nameHospital))
                query = query.Where(e => e.Hospital != null && e.Hospital.Name!.Contains(nameHospital));

            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(e => e.Specialization == specialization);

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> GetClinicCountAsync(string? nameClinic, string? nameHospital, string? specialization)
        {
            var query = _context.Clinics.Include(c => c.Hospital).AsQueryable();

            if (!string.IsNullOrWhiteSpace(nameClinic))
                query = query.Where(e => e.Name!.Contains(nameClinic));

            if (!string.IsNullOrWhiteSpace(nameHospital))
                query = query.Where(e => e.Hospital != null && e.Hospital.Name!.Contains(nameHospital));

            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(e => e.Specialization == specialization);

            return await query.CountAsync();
        }

        public async Task<Clinic?> GetClinicByIdAsync(int id)
        {
            return await _context.Clinics.Include(c => c.Hospital).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddClinicAsync(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClinicAsync(Clinic clinic)
        {
            _context.Clinics.Update(clinic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClinicAsync(Clinic clinic)
        {
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ClinicExistsAsync(int id)
        {
            return await _context.Clinics.AnyAsync(e => e.Id == id);
        }
    }
}
