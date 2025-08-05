using AutoMapper;
using Clinic_Complex_Management_System.DTOs.Clinic;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfile
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile()
        {
            CreateMap<Clinic, ClinicDto>()
                .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.Name : null));

            CreateMap<CreateClinicDto, Clinic>();
            CreateMap<UpdateClinicDto, Clinic>();
        }
    }
}