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

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var doctor = await _repository.GetDoctorByIdAsync(id);
            return doctor?.Adapt<DoctorDto>();
        }

        public async Task<string> AddDoctorAsync(CreateDoctorDto dto)
        {
            var doctor = dto.Adapt<Doctor>();
            if (dto.Image is not null && dto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctor", filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = File.Create(path);
                await dto.Image.CopyToAsync(stream);
                doctor.images = filename;
            }
            await _repository.AddDoctorAsync(doctor);
            return "Doctor added successfully.";
        }

        public async Task<string> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var existing = await _repository.GetDoctorByIdAsync(id);
            if (existing is null) return "Doctor not found.";

            var doctor = dto.Adapt<Doctor>();
            doctor.Id = id;

            if (dto.Image is not null && dto.Image.Length > 0)
            {
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctor", filename);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using var stream = File.Create(path);
                await dto.Image.CopyToAsync(stream);

                // delete old image
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctor", existing.images ?? "");
                if (File.Exists(oldPath)) File.Delete(oldPath);

                doctor.images = filename;
            }
            else
            {
                doctor.images = existing.images;
            }

            await _repository.UpdateDoctorAsync(doctor);
            return "Doctor updated successfully.";
        }

        public async Task<string> DeleteDoctorAsync(int id)
        {
            var doctor = await _repository.GetDoctorByIdAsync(id);
            if (doctor == null) return "Doctor not found.";

            // delete image
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/doctor", doctor.images ?? "");
            if (File.Exists(path)) File.Delete(path);

            await _repository.DeleteDoctorAsync(doctor);
            return "Doctor deleted successfully.";
        }
    }
}
