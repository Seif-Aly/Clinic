using Clinic_Complex_Management_System.DTOs.Clinic;

namespace Clinic_Complex_Management_System1.DTOs.Doctor
{
    public class GetClinicsResult
    {
        public List<ClinicDto> Clinics { get; set; } = new List<ClinicDto>();
        public int TotalCount { get; set; }
    }
}
