namespace Clinic_Complex_Management_System1.Models
{
    public class PrescriptionItem
    {
        public int Id { get; set; }
        public string ?MedicineName { get; set; }
        public string? Dosage { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription? Prescription { get; set; }
    }
}
