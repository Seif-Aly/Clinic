namespace Clinic_Complex_Management_System1.DTOs.Doctor
{
    public class CreateDoctorDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Specialization { get; set; }
        public int ClinicId { get; set; }
    }
}