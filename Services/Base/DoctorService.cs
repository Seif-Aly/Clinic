using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Clinic_Complex_Management_System.Data;
using Microsoft.EntityFrameworkCore;


namespace Clinic_Complex_Management_System1.Services.Base
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;

        private readonly AppDbContext _context;

        public DoctorService(IDoctorRepository repository,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            AppDbContext context
            )
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<GetDoctorsResult> GetDoctorsAsync(DoctorFilterRequest? filter, int page)
        {
            var doctors = await _repository.GetDoctorsAsync(filter?.NameDoctor, filter?.NameClinic, filter?.Specialization, page);
            var total = await _repository.GetTotalDoctorsCountAsync(filter?.NameDoctor, filter?.NameClinic, filter?.Specialization);
            var result = new GetDoctorsResult
            {
                Doctors = _mapper.Map<List<DoctorDto>>(doctors),
                TotalCount = total
            };
            return result;
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctorEntity = await _repository.GetDoctorByIdAsync(id);
            var doctorDto = _mapper.Map<DoctorDto>(doctorEntity);
            return doctorDto;
        }

        public async Task<DoctorDto> AddDoctorAsync(CreateDoctorDto doctorDto)
        {
            var user = new User
            {
                UserName = doctorDto.Email,
                Email = doctorDto.Email,
                EmailConfirmed = true
            };
            var createUserResult = await _userManager.CreateAsync(user, "Doctor@1234");
            
            if (!createUserResult.Succeeded)
                return null;

            await _userManager.AddToRoleAsync(user, "Doctor");
            var doctor = doctorDto.Adapt<Doctor>();
            doctor.UserId = user.Id;

            try
            {
                if (doctorDto.Image != null && doctorDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(doctorDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await doctorDto.Image.CopyToAsync(stream);
                    }
                    doctor.images = filename; // نخزن اسم الملف فقط
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            await _repository.AddDoctorAsync(doctor);
            var result = _mapper.Map<DoctorDto>(doctor);
            return result;
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            await _repository.UpdateDoctorAsync(doctor);
        }

        // public async Task<bool> DeleteDoctorAsync(int id)
        // {
        //     var doctor = await _repository.GetDoctorByIdAsync(id);
        //     if (doctor == null) return false;

        //     await _repository.DeleteDoctorAsync(doctor);
        //     //remove user for deleted doctor
        //     if (doctor.UserId != null)
        //     {
        //         var user = await _userManager.FindByIdAsync(doctor.UserId.ToString());
        //         if (user != null)
        //         {
        //             var roles = await _userManager.GetRolesAsync(user);
        //             await _userManager.RemoveFromRolesAsync(user, roles);
        //             await _userManager.DeleteAsync(user);
        //         }
        //     }
        //     return true;
        // }

        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            // Step 1: Remove appointments
            var appointments = _context.Appointments.Where(a => a.DoctorId == doctorId);
            _context.Appointments.RemoveRange(appointments);

            // Step 2: Remove prescriptions
            var prescriptions = _context.Prescriptions.Where(p => p.DoctorId == doctorId);
            _context.Prescriptions.RemoveRange(prescriptions);

            // Step 3: Remove doctor
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;
            _context.Doctors.Remove(doctor);

            // Step 4: Remove user (if connected)
            if (doctor.UserId != null)
            {
                var user = await _userManager.FindByIdAsync(doctor.UserId.ToString());
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.DeleteAsync(user);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public Task<int> GetDoctorIdByUserIdAsync(Guid userId)
        {
            return _repository.GetDoctorIdByUserIdAsync(userId);
        }

        public async Task<bool> UpdateDoctorAsync(UpdateDoctorDto updateDoctorDto, int doctorId)
        {

            try
            {
                var doctorInDb = await _repository.GetDoctorByIdAsync(doctorId);
                if (doctorInDb == null)
                    return false;

                doctorInDb.FullName = updateDoctorDto.FullName ?? doctorInDb.FullName;
                doctorInDb.Email = updateDoctorDto.Email ?? doctorInDb.Email;
                doctorInDb.Phone = updateDoctorDto.Phone ?? doctorInDb.Phone;
                doctorInDb.Specialization = updateDoctorDto.Specialization ?? doctorInDb.Specialization;
                doctorInDb.ClinicId = updateDoctorDto.ClinicId ?? doctorInDb.ClinicId;
                if (!string.IsNullOrEmpty(updateDoctorDto.Email))
                {
                    if (doctorInDb.UserId != null)
                    {
                        var user = await _userManager.FindByIdAsync(doctorInDb.UserId.ToString());
                        if (user != null)
                        {
                            await _userManager.SetEmailAsync(user, updateDoctorDto.Email);
                            await _userManager.SetUserNameAsync(user, updateDoctorDto.Email);
                        }
                    }
                }

                // التعامل مع الصورة
                if (updateDoctorDto.Image != null && updateDoctorDto.Image.Length > 0)
                {
                    var filename = Guid.NewGuid().ToString() + Path.GetExtension(updateDoctorDto.Image.FileName);
                    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", filename);
                    using (var stream = System.IO.File.Create(filepath))
                    {
                        await updateDoctorDto.Image.CopyToAsync(stream);
                    }

                    // حذف الصورة القديمة
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\doctor", doctorInDb.images);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }

                    doctorInDb.images = filename; // نخزن اسم الملف مش المسار الكامل
                }
                else
                {
                    doctorInDb.images = doctorInDb.images;
                }

                await _repository.UpdateDoctorAsync(doctorInDb);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }
    }
}
