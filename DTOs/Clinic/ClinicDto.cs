namespace Clinic_Complex_Management_System.DTOs.Clinic
{
    public class ClinicDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }
        public string? Image { get; set; }

        public string? HospitalName { get; set; } // from Hospital.Name
    }
}