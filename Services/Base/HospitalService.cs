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
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> UpdateHospitalAsync(Hospital hospital)
        {
            _repository.UpdateHospital(hospital);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteHospitalAsync(int id)
        {
            var hospital = await _repository.GetHospitalByIdAsync(id);
            if (hospital == null) return false;

            _repository.DeleteHospital(hospital);
            return await _repository.SaveChangesAsync();
        }
    }

}
