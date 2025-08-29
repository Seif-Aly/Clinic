namespace Clinic_Complex_Management_System1.Models
{
    public class Prescription
    {
        public int Id { get; set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime DateIssued { get; set; }

        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }
        public int? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }


        public ICollection<PrescriptionItem>? PrescriptionItems { get; set; }
    }

}
