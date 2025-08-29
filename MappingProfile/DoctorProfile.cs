using AutoMapper;
using Clinic_Complex_Management_System1.DTOs.Doctor;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            // Doctor => DoctorDto
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : null))
                .ReverseMap();

            // CreateDoctorDto => Doctor
            CreateMap<CreateDoctorDto, Doctor>().ReverseMap();

            // UpdateDoctorDto => Doctor
            CreateMap<UpdateDoctorDto, Doctor>().ReverseMap();
        }
    }
}
