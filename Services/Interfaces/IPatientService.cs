using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;

public interface IPatientService
{
    Task<GetPatientsResult> GetPatientsAsync(PatientFilterRequest? filter, int page, int pageSize);
    Task<Patient?> GetPatientByIdAsync(int id);
    Task<bool> CreatePatientAsync(Patient patient);
    Task<bool> UpdatePatientAsync(Patient patient);
    Task<bool> DeletePatientAsync(int id);
}
