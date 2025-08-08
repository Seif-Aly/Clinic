namespace Clinic_Complex_Management_System.DTOs.Clinic
{
    public class UpdateClinicDto
    {
       public int Id { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }
        public int HospitalId { get; set; }
        public IFormFile? Image { get; set; }
    }
}