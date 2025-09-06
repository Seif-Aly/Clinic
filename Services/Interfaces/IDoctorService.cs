using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<GetDoctorsResult> GetDoctorsAsync(DoctorFilterRequest? filter, int page);
        Task<DoctorDto?> GetDoctorByIdAsync(int id);
        Task<int> GetDoctorIdByUserIdAsync(Guid userId);

        Task<bool> DeleteDoctorAsync(int id);
        Task<DoctorDto> AddDoctorAsync(CreateDoctorDto doctor);
        Task<bool> UpdateDoctorAsync(UpdateDoctorDto doctor, int doctorId);
        Task UpdateDoctorAsync(Doctor doctorInDb);
    }
}
