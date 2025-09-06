using AutoMapper;
using Clinic_Complex_Management_System.DTOs.Patient;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfile
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<Patient, RegisterPatientResult>()
               .ReverseMap();
            CreateMap<CreatePatientDto, Patient>().ReverseMap();
            CreateMap<UpdatePatientDto, Patient>().ReverseMap();
        }
    }
}