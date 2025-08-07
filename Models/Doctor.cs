namespace Clinic_Complex_Management_System1.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialization { get; set; }

        public int? ClinicId { get; set; }
        public Clinic? Clinic { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }

}
