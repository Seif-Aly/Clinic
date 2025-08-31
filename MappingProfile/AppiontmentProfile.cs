using AutoMapper;
using Clinic_Complex_Management_System.DTOs.Appointment;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfile
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(d => d.DoctorName,
                opt => opt.MapFrom(s =>
                    s.Doctor != null && !string.IsNullOrWhiteSpace(s.Doctor.FullName)
                        ? s.Doctor.FullName
                        : s.Doctor != null ? s.Doctor.Email : string.Empty))
            .ForMember(d => d.PatientName,
                opt => opt.MapFrom(s =>
                    s.Patient != null && !string.IsNullOrWhiteSpace(s.Patient.FullName)
                        ? s.Patient.FullName
                        : s.Patient != null ? s.Patient.Email : string.Empty));

            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>();
        }
    }
}