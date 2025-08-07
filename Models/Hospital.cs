namespace Clinic_Complex_Management_System1.Models
{
    public class Hospital
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public ICollection<Clinic>? Clinics { get; set; }
    }

}
