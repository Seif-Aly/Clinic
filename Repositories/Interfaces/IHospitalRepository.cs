using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Repositories.Interfaces
{
    public interface IHospitalRepository
    {
        Task<IEnumerable<Hospital>> GetHospitalsAsync(HospitaliFilterRequest? filter, int page);
        Task<Hospital?> GetHospitalByIdAsync(int id);
        Task AddHospitalAsync(Hospital hospital);
        void UpdateHospital(Hospital hospital);
        void DeleteHospital(Hospital hospital);
        Task<bool> SaveChangesAsync();
        Task<int> GetTotalCountAsync(HospitaliFilterRequest? filter);
    }

}
