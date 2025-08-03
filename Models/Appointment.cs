namespace Clinic_Complex_Management_System1.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ?Status { get; set; }

        public int DoctorId { get; set; }
        public Doctor ?Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient ?Patient { get; set; }

        public Prescription? Prescription { get; set; }
    }

}
