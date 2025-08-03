namespace Clinic_Complex_Management_System.DTOs.PrescriptionItem
{
    public class CreatePrescriptionItemDto
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        public int PrescriptionId { get; set; }
    }
}