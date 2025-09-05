using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;

namespace Clinic_Complex_Management_System1.Services.Base
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;

        public DoctorService(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetDoctorsResult> GetDoctorsAsync(DoctorFilterRequest? filter, int page)
        {
            var doctors = await _repository.GetDoctorsAsync(filter?.NameDoctor, filter?.NameClinic, filter?.Specialization, page);
            var total = await _repository.GetTotalDoctorsCountAsync(filter?.NameDoctor, filter?.NameClinic, filter?.Specialization);
            var result = new GetDoctorsResult
            {
                Doctors = doctors.Adapt<List<DoctorDto>>(),
                TotalCount = total
            };
            return result;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
           
            return await _repository.GetDoctorByIdAsync(id);
        }

        public async Task<bool> AddDoctorAsync(Doctor doctor)
        {
          
            await _repository.AddDoctorAsync(doctor);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateDoctorAsync(Doctor doctor)
        {
           
          await  _repository.UpdateDoctorAsync(doctor);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteDoctorAsync(int id)
        {
         
            var hospital = await _repository.GetDoctorByIdAsync(id);
            if (hospital == null) return false;

           await _repository.DeleteDoctorAsync(hospital);
            return await _repository.SaveChangesAsync();
        }
    }
}
