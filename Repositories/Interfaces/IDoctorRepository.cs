using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetDoctorsAsync(string? name, string? clinic, string? specialization, int page);
        Task<int> GetTotalDoctorsCountAsync(string? name, string? clinic, string? specialization);
        Task<Doctor?> GetDoctorByIdAsync(int id);
        Task<int> GetDoctorIdByUserIdAsync(Guid userId);
        Task AddDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task DeleteDoctorAsync(Doctor doctor);
    }
}
