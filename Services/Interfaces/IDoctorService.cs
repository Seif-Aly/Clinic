using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<(List<DoctorDto>, int)> GetDoctorsAsync(DoctorFilterRequest? filter, int page);
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task<string> AddDoctorAsync(CreateDoctorDto dto);
        Task<string> UpdateDoctorAsync(int id, UpdateDoctorDto dto);
        Task<string> DeleteDoctorAsync(int id);
    }
}
