using Clinic_Complex_Management_System1.Models;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<Patient?> GetByIdAsync(int id);
    Task AddAsync(Patient patient);
    void Update(Patient patient);
    void Remove(Patient patient);
    Task<bool> SaveChangesAsync();
}
