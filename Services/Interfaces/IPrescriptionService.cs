using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.ViewModels;

public interface IPrescriptionService
{
    Task<(IEnumerable<Prescription> prescriptions, int totalPages)> GetPrescriptionsAsync(PrescriptionFilterRequest? filter, int page, string? role, int? doctorId, int? patientId);
    Task<Prescription?> GetPrescriptionByIdAsync(int id, string? role, int? doctorId, int? patientId);
    Task<bool> CreatePrescriptionAsync(PrescriptionWithItemsVM model, string? role, int? doctorId);
    Task<bool> UpdatePrescriptionAsync(int id, PrescriptionWithItemsVM model, string? role, int? doctorId);
    Task<bool> DeletePrescriptionAsync(int id, string? role, int? doctorId);
    Task<IEnumerable<Prescription>> GetPrescriptionsByPatientAsync(int patientId, string? role, int? currentPatientId);
    Task<IEnumerable<Prescription>> GetPrescriptionsByDoctorAsync(int doctorId, string? role, int? currentDoctorId);
}
