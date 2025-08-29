namespace Clinic_Complex_Management_System1.DTOs.Doctor
{
    public class GetDoctorsResult
    {
        public List<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
        public int TotalCount { get; set; }
    }
}
