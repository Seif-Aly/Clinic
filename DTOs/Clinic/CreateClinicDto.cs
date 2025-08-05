namespace Clinic_Complex_Management_System.DTOs.Clinic
{
    public class CreateClinicDto
    {
        public string? Name { get; set; }
        public string? Specialization { get; set; }
        public int HospitalId { get; set; }
    }
}