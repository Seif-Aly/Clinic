using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Complex_Management_System.Repositories
{
    public class PrescriptionItemRepository : IPrescriptionItemRepository
    {
        private readonly AppDbContext _context;

        public PrescriptionItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PrescriptionItem>> GetAllAsync()
        {
            return await _context.PrescriptionItems.Include(p => p.Prescription).ToListAsync();
        }

        public async Task<PrescriptionItem?> GetByIdAsync(int id)
        {
            return await _context.PrescriptionItems.Include(p => p.Prescription)
                                                   .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(PrescriptionItem item)
        {
            await _context.PrescriptionItems.AddAsync(item);
        }

        public void Update(PrescriptionItem item)
        {
            _context.PrescriptionItems.Update(item);
        }

        public void Remove(PrescriptionItem item)
        {
            _context.PrescriptionItems.Remove(item);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
