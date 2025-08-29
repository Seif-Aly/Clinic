using AutoMapper;
using Clinic_Complex_Management_System1.DTOs.User;
using Clinic_Complex_Management_System1.Models;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
    }
}