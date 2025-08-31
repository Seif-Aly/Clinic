using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System1.Repositories.Base
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Doctor>> GetDoctorsAsync(string? name, string? clinic, string? specialization, int page)
        {
            var query = _context.Doctors.Include(d => d.Clinic).Include(d => d.Appointments).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.FullName.Contains(name));
            if (!string.IsNullOrWhiteSpace(clinic))
                query = query.Where(d => d.Clinic != null && d.Clinic.Name.Contains(clinic));
            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(d => d.Specialization.Contains(specialization));

            return await query.Skip((page - 1) * 6).Take(6).ToListAsync();
        }

        public async Task<int> GetTotalDoctorsCountAsync(string? name, string? clinic, string? specialization)
        {
            var query = _context.Doctors.Include(d => d.Clinic).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(d => d.FullName.Contains(name));
            if (!string.IsNullOrWhiteSpace(clinic))
                query = query.Where(d => d.Clinic != null && d.Clinic.Name.Contains(clinic));
            if (!string.IsNullOrWhiteSpace(specialization))
                query = query.Where(d => d.Specialization.Contains(specialization));

            return await query.CountAsync();
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors.Include(d => d.Clinic).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
<<<<<<< HEAD
            _context.Doctors.Update(doctor);
=======
           var existing = await _context.Doctors.FindAsync(doctor.Id);
            if (existing != null)
            {
                _context.Entry(existing).State = EntityState.Detached;
            }

            _context.Doctors.Update(doctor);

>>>>>>> main
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
        }
    }
}
