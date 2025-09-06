namespace Clinic_Complex_Management_System1.ViewModels
{
    public class PrescriptionWithItemsVM
    {
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
        public DateTime DateIssued { get; set; }
        public int AppointmentId { get; set; }
        public List<PrescriptionItemVM>? Items { get; set; }
    }

    public class PrescriptionItemVM
    {
        public string? MedicationName { get; set; }
        public string? Dosage { get; set; }
        public string? Instructions { get; set; }
    }
}
