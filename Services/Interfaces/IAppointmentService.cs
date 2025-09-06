using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Appointment;
using Clinic_Complex_Management_System1.DTOs.Appointment;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<GetAppointmentsResult> GetAppointments(AppointmantFilterReqest? filter, string role, int? doctorId, int? patientId, int page);
        Task<AppointmentDto?> GetAppointment(int id, string role, int? doctorId, int? patientId);
        Task<AppointmentDto?> CreateAppointment(CreateAppointmentDto dto, string role, int? doctorId, int? patientId);
        Task<bool> UpdateAppointment(int id, UpdateAppointmentDto dto, string role, int? doctorId, int? patientId);
        Task<bool> DeleteAppointment(int id);
    }

}
