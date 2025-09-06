using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Repositories.Interfaces
{
    public interface IProfileUserRepository
    {
        Task<IEnumerable<Profileuser>> GetAllAsync();
        Task<Profileuser?> GetByIdAsync(int id);
        Task AddAsync(Profileuser profile);
        void Update(Profileuser profile);
        void Delete(Profileuser profile);
        Task<bool> SaveChangesAsync();
    }
}
