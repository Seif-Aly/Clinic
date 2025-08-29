using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;

public interface IPrescriptionRepository
{
    Task<(IEnumerable<Prescription> prescriptions, int totalPages)> GetPrescriptionsAsync(PrescriptionFilterRequest? filter, int page, string? role, int? doctorId, int? patientId);
    Task<Prescription?> GetPrescriptionByIdAsync(int id);
    Task AddPrescriptionAsync(Prescription prescription);
    Task UpdatePrescriptionAsync(Prescription prescription);
    Task DeletePrescriptionAsync(Prescription prescription);
    Task<bool> PrescriptionExistsAsync(int id);
}
