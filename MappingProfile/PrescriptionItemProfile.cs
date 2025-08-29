using AutoMapper;
using Clinic_Complex_Management_System.DTOs.PrescriptionItem;
using Clinic_Complex_Management_System1.Models;

namespace Clinic_Complex_Management_System.MappingProfiles
{
    public class PrescriptionItemProfile : Profile
    {
        public PrescriptionItemProfile()
        {
            CreateMap<PrescriptionItem, PrescriptionItemDto>();
            CreateMap<CreatePrescriptionItemDto, PrescriptionItem>();
            CreateMap<UpdatePrescriptionItemDto, PrescriptionItem>();
        }
    }
}