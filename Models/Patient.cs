namespace Clinic_Complex_Management_System1.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? NationalId { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid? UserId { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
        public User? User { get; set; }
    }

}
