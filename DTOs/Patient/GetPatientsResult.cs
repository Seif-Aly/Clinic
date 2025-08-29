namespace Clinic_Complex_Management_System1.DTOs.Patient
{
    public class GetPatientsResult
    {
        public IEnumerable<Models.Patient> Patients { get; set; } = null!;
        public int TotalPages { get; set; }
    }
}
