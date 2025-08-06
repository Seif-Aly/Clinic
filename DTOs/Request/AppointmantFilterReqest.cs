namespace Clinic_Complex_Management_System.DTos.Request
{
  
    public class AppointmantFilterReqest
    {
        public string? NameDoctor { get; set; }
        public string? stutas { get; set; }
        public DateTime? date { get; set; }
        public string? Specialization { get; set; }
    }
}
