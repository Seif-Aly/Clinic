using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IHospitalService
    {
        Task<GetHospitalsResult> GetHospitalsAsync(HospitaliFilterRequest? filter, int page);
        Task<Hospital?> GetHospitalByIdAsync(int id);
        Task<bool> AddHospitalAsync(Hospital hospital);
        Task<bool> UpdateHospitalAsync(Hospital hospital);
        Task<bool> DeleteHospitalAsync(int id);
    }

}
