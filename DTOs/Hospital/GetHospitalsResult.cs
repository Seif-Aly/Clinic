using Clinic_Complex_Management_System.DTOs.Hospital;

namespace Clinic_Complex_Management_System1.DTOs.Hospital
{
    public class GetHospitalsResult
    {
        public List<HospitalDto> Hospitals { get; set; } = new List<HospitalDto>();
        public int TotalCount { get; set; }
    }
}
