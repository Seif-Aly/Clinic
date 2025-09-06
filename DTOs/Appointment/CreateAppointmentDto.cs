namespace Clinic_Complex_Management_System.DTOs.Appointment
{
    public class CreateAppointmentDto
    {
        public DateTime AppointmentDateTime { get; set; }
        public string? Status { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}