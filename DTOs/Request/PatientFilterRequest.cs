namespace Clinic_Complex_Management_System.DTos.Request
{
    public class PatientFilterRequest
    {
      public  string? NamePationt { get; set; }
        public string? gender { get; set; }
        public DateTime ? dateOfBrith { get;set; }
        public string? National { get;set; }
    }
}
