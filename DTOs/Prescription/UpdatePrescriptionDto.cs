namespace Clinic_Complex_Management_System.DTOs.Prescription
{
    public class UpdatePrescriptionDto
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime DateIssued { get; set; }

        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public object Items { get; internal set; }
    }
}