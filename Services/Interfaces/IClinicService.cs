using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.DTOs.Doctor;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IClinicService
    {
        Task<GetClinicsResult> GetClinicsAsync(ClinicFilterRequest? filter, int page);
        Task<ClinicDto?> GetClinicByIdAsync(int id);
        Task<string> AddClinicAsync(CreateClinicDto dto);
        Task<string> UpdateClinicAsync(int id, UpdateClinicDto dto);
        Task<string> DeleteClinicAsync(int id);
    }
}
