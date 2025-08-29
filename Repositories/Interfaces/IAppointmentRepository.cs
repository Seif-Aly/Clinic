using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetFilteredAppointmentsAsync(AppointmantFilterReqest? filter, string? role, int? doctorId, int? patientId, int page);
        Task<int> CountFilteredAppointmentsAsync(AppointmantFilterReqest? filter, string? role, int? doctorId, int? patientId);
        Task<Appointment?> GetByIdAsync(int id);
        Task AddAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
    }

}
