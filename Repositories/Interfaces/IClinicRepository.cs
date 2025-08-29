using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Repositories.Interfaces
{
    public interface IClinicRepository
    {
        Task<List<Clinic>> GetClinicsAsync(string? nameClinic, string? nameHospital, string? specialization, int page, int pageSize);
        Task<int> GetClinicCountAsync(string? nameClinic, string? nameHospital, string? specialization);
        Task<Clinic?> GetClinicByIdAsync(int id);
        Task AddClinicAsync(Clinic clinic);
        Task UpdateClinicAsync(Clinic clinic);
        Task DeleteClinicAsync(Clinic clinic);
        Task<bool> ClinicExistsAsync(int id);
    }
}


