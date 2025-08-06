namespace Clinic_Complex_Management_System1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; } 
        public string? Role { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}
