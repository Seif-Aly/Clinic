namespace Clinic_Complex_Management_System1.Models
{
    public class Profileuser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public Guid UserId { get; set; }   // FK
        public User User { get; set; }     // Navigation
    }
}
