using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Models;
using Clinic_Complex_Management_System1.Repositories.Interfaces;
using Clinic_Complex_Management_System1.Services.Interfaces;
using Mapster;

namespace Clinic_Complex_Management_System1.Services.Base
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _repository;

        public ClinicService(IClinicRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<ClinicDto>, int)> GetClinicsAsync(ClinicFilterRequest? filter, int page)
        {
            int pageSize = 6;
            var clinics = await _repository.GetClinicsAsync(filter?.NameClinic, filter?.NmaeHospitale, filter?.specialization, page, pageSize);
            var totalCount = await _repository.GetClinicCountAsync(filter?.NameClinic, filter?.NmaeHospitale, filter?.specialization);
            var dtos = clinics.Adapt<List<ClinicDto>>();
            return (dtos, totalCount);
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

            var updatedClinic = dto.Adapt<Clinic>();
            updatedClinic.Id = id;

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

                updatedClinic.Image = filename;
            }
            else
            {
                updatedClinic.Image = existingClinic.Image;
            }

            await _repository.UpdateClinicAsync(updatedClinic);
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
