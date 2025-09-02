using Clinic_Complex_Management_System.DTos.Request;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System1.Services.Interfaces
{
    public interface IDoctorService
    {
        Task<GetDoctorsResult> GetDoctorsAsync(DoctorFilterRequest? filter, int page);
        Task<Doctor?> GetDoctorByIdAsync(int id);
        Task<bool> DeleteDoctorAsync(int id);
        Task<bool> AddDoctorAsync(Doctor doctors);
        Task<bool> UpdateDoctorAsync(Doctor doctorInDb);
    }
}
