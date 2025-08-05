namespace Clinic_Complex_Management_System.DTOs.Prescription
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime DateIssued { get; set; }

        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
        public int AppointmentId { get; set; }
    }
}