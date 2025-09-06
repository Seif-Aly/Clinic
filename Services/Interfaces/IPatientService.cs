using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;

public interface IPatientService
{
    Task<GetPatientsResult> GetPatientsAsync(PatientFilterRequest? filter, int page, int pageSize);
    Task<PatientDto?> GetPatientByIdAsync(int id);
    Task<int> GetPatientIdByUserIdAsync(Guid userId);
    Task<PatientDto> CreatePatientAsync(CreatePatientDto patient);
    Task<bool> UpdatePatientAsync(Patient patient);
    Task<bool> UpdatePatientAsync(UpdatePatientDto patientDto, int id);
    Task<bool> DeletePatientAsync(int id);
}
