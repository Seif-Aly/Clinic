using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<(IEnumerable<AppointmentDto>, int totalPages)> GetAppointments(AppointmantFilterReqest? filter, string role, int? doctorId, int? patientId, int page);
        Task<AppointmentDto?> GetAppointment(int id, string role, int? doctorId, int? patientId);
        Task<AppointmentDto?> CreateAppointment(CreateAppointmentDto dto, string role, int? doctorId);
        Task<bool> UpdateAppointment(UpdateAppointmentDto dto);
        Task<bool> DeleteAppointment(int id);
    }

}
