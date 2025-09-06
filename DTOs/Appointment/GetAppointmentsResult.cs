using Clinic_Complex_Management_System.DTOs.Appointment;

namespace Clinic_Complex_Management_System1.DTOs.Appointment
{
    public class GetAppointmentsResult
    {
        public List<AppointmentDto> Appointments { get; set; }
        public int TotalCount { get; set; }

    }
}
