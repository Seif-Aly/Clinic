using AutoMapper;
using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;

namespace Clinic_Complex_Management_System1.Services.Base
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _repository;
        private readonly IMapper _mapper;

        public ClinicService(IClinicRepository repository,
            IMapper mapper
            )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetClinicsResult> GetClinicsAsync(ClinicFilterRequest? filter, int page)
        {

            int pageSize = 6;
            var clinics = await _repository.GetClinicsAsync(filter?.NameClinic, filter?.NmaeHospitale, filter?.specialization, page, pageSize);
            var totalCount = await _repository.GetClinicCountAsync(filter?.NameClinic, filter?.NmaeHospitale, filter?.specialization);
            var result = new GetClinicsResult
            {
                Clinics = _mapper.Map<List<ClinicDto>>(clinics),
                TotalCount = totalCount
            };
            return result;
        }

        public async Task<ClinicDto?> GetClinicByIdAsync(int id)
        {
            var clinic = await _repository.GetClinicByIdAsync(id);
            return clinic?.Adapt<ClinicDto>();
        }

        public async Task<string> AddClinicAsync(CreateClinicDto dto)
        {
            var clinic = dto.Adapt<Clinic>();
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clinic", filename);
                using (var stream = File.Create(filepath))
                {
                    await dto.Image.CopyToAsync(stream);
                }
                clinic.Image = filename;
            }
            await _repository.AddClinicAsync(clinic);
            return "Successfully added clinic.";
        }

        public async Task<string> UpdateClinicAsync(int id, UpdateClinicDto dto)
        {
            var existingClinic = await _repository.GetClinicByIdAsync(id);
            if (existingClinic == null)
                return "Clinic not found.";
            existingClinic.Name = dto.Name ?? existingClinic.Name;
            existingClinic.Specialization = dto.Specialization ?? existingClinic.Specialization;
            existingClinic.HospitalId = dto.HospitalId != 0 ? dto.HospitalId : existingClinic.HospitalId;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var filename = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clinic", filename);
                using (var stream = File.Create(filepath))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingClinic.Image))
                {
                    var oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clinic", existingClinic.Image);
                    if (File.Exists(oldFile)) File.Delete(oldFile);
                }

                existingClinic.Image = filename;
            }
            else
            {
                existingClinic.Image = existingClinic.Image;
            }

            await _repository.UpdateClinicAsync(existingClinic);
            return "Clinic updated successfully.";
        }

        public async Task<string> DeleteClinicAsync(int id)
        {
            var clinic = await _repository.GetClinicByIdAsync(id);
            if (clinic == null) return "Clinic not found.";

            if (!string.IsNullOrEmpty(clinic.Image))
            {
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/clinic", clinic.Image);
                if (File.Exists(filepath)) File.Delete(filepath);
            }

            await _repository.DeleteClinicAsync(clinic);
            return "Clinic deleted successfully.";
        }
    }
}
