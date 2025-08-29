namespace Clinic_Complex_Management_System1.DTOs.Doctor
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialization { get; set; }
        public string? Image { get; set; }

        public string? ClinicName { get; set; } // from clinic
    }
}