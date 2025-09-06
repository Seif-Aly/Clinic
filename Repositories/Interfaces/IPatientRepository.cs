using Clinic_Complex_Management_System1.Models;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task<int> GetPatientIdByUserIdAsync(Guid userId);

    Task AddAsync(Patient patient);
    Task<bool> Update(Patient patient);
    Task<bool> Remove(Patient patient);
}
