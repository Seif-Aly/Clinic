using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.Repositories
{
    public interface IPrescriptionItemRepository
    {
        Task<IEnumerable<PrescriptionItem>> GetAllAsync();
        Task<PrescriptionItem?> GetByIdAsync(int id);
        Task AddAsync(PrescriptionItem item);
        void Update(PrescriptionItem item);
        void Remove(PrescriptionItem item);
        Task<bool> SaveChangesAsync();
    }
}
