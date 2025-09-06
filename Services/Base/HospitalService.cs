using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;

namespace Clinic_Complex_Management_System1.Services.Base
{
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _repository;

        public HospitalService(IHospitalRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetHospitalsResult> GetHospitalsAsync(HospitaliFilterRequest? filter, int page)
        {
            var hospitals = await _repository.GetHospitalsAsync(filter, page);
            var totalCount = await _repository.GetTotalCountAsync(filter);

            var result = new GetHospitalsResult
            {
                Hospitals = hospitals.Adapt<List<HospitalDto>>(),
                TotalCount = totalCount
            };
            return result;
        }


        public async Task<Hospital?> GetHospitalByIdAsync(int id)
        {
            return await _repository.GetHospitalByIdAsync(id);
        }

        public async Task<bool> AddHospitalAsync(Hospital hospital)
        {
            await _repository.AddHospitalAsync(hospital);
            return true;
        }

        public async Task<bool> UpdateHospitalAsync(Hospital hospital)
        {
            await _repository.UpdateHospital(hospital);
            return true;
        }

        public async Task<bool> DeleteHospitalAsync(int id)
        {
            var hospital = await _repository.GetHospitalByIdAsync(id);
            if (hospital == null) return false;

            await _repository.DeleteHospital(hospital);
            return true;
        }

        public async Task<string> UpdateHospitalAsync(UpdateHospitalDto dto, int id)
        {
            var existingHospital = await _repository.GetHospitalByIdAsync(id);
            if (existingHospital == null)
                return "Hospital not found.";

            existingHospital.Name = dto.Name ?? existingHospital.Name;
            existingHospital.Address = dto.Address ?? existingHospital.Address;
            existingHospital.Phone = dto.Phone ?? existingHospital.Phone;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/hospital", filename);
                using (var stream = File.Create(filepath))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingHospital.Image))
                {
                    var oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/hospital", existingHospital.Image);
                    if (File.Exists(oldFile)) File.Delete(oldFile);
                }

                existingHospital.Image = filename;
            }
            else
            {
                existingHospital.Image = existingHospital.Image;
            }

            await _repository.UpdateHospital(existingHospital);
            return "Hospital updated successfully.";
        }
    }

}
