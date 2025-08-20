using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System1.Repositories.Base
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetFilteredAppointmentsAsync(AppointmantFilterReqest? filter, string? role, int? doctorId, int? patientId, int page)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsQueryable();

            if (role == "Patient" && patientId != null)
                query = query.Where(a => a.PatientId == patientId.Value);
            else if (role == "Doctor" && doctorId != null)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (!string.IsNullOrEmpty(filter?.NameDoctor))
                query = query.Where(a => a.Doctor.FullName.Contains(filter.NameDoctor));
            if (!string.IsNullOrEmpty(filter?.Specialization))
                query = query.Where(a => a.Doctor.Specialization.Contains(filter.Specialization));
            if (filter?.date != null)
                query = query.Where(a => a.AppointmentDateTime.Date == filter.date.Value.Date);
            if (filter?.status != null)
                query = query.Where(a => a.Status == filter.status);

            return await query
                .OrderByDescending(a => a.AppointmentDateTime)
                .Skip((page - 1) * 6)
                .Take(6)
                .ToListAsync();
        }

        public async Task<int> CountFilteredAppointmentsAsync(AppointmantFilterReqest? filter, string? role, int? doctorId, int? patientId)
        {
            var query = _context.Appointments.AsQueryable();

            if (role == "Patient" && patientId != null)
                query = query.Where(a => a.PatientId == patientId.Value);
            else if (role == "Doctor" && doctorId != null)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (!string.IsNullOrEmpty(filter?.NameDoctor))
                query = query.Where(a => a.Doctor.FullName.Contains(filter.NameDoctor));
            if (!string.IsNullOrEmpty(filter?.Specialization))
                query = query.Where(a => a.Doctor.Specialization.Contains(filter.Specialization));
            if (filter?.date != null)
                query = query.Where(a => a.AppointmentDateTime.Date == filter.date.Value.Date);
            if (filter?.status != null)
                query = query.Where(a => a.Status == filter.status);

            return await query.CountAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }
    }

}
