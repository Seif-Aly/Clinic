namespace Clinic_Complex_Management_System.DTos.Request
{
    public class PrescriptionFilterRequest
    {
        public int? DoctorId { get; set; }
        public string? NameDoctor { get; set; }
        public int? PationtId { get; set; }
    
        public int? AppointmantId { get; set; }
        public DateTime? DateIssued { get; set; }
    }
}
