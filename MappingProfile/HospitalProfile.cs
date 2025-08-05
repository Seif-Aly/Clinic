using AutoMapper;
using Clinic_Complex_Management_System.DTOs.Hospital;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfile
{
    public class HospitalProfile : Profile
    {
        public HospitalProfile()
        {
            CreateMap<Hospital, HospitalDto>();
            CreateMap<CreateHospitalDto, Hospital>();
            CreateMap<UpdateHospitalDto, Hospital>();
        }
    }
}