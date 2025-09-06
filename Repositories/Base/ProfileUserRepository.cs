using Clinic_Complex_Management_System.Data;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Clinic_Complex_Management_System1.Repositories.Base
{
    public class ProfileUserRepository: IProfileUserRepository
    {
        private readonly AppDbContext _context;

        public ProfileUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Profileuser>> GetAllAsync()
        {
          return await _context.profileusers.ToListAsync();
        }
           

        public async Task<Profileuser?> GetByIdAsync(int id)
        {
           return  await _context.profileusers.FindAsync(id);
        }


        public async Task AddAsync(Profileuser profile)
        {
            await _context.profileusers.AddAsync(profile);

        }

        public void Update(Profileuser profile)
        {
             _context.profileusers.Update(profile);

        }

        public void Delete(Profileuser profile)
        {
           
             _context.profileusers.Remove(profile);
        }

  
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
               await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
               Console.WriteLine($"errores:{ex}");
                return false;
            }
        }
    }
}
