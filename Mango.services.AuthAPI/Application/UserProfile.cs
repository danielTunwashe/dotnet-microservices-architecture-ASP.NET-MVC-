using AutoMapper;
using Mango.services.AuthAPI.Models;
using Mango.services.AuthAPI.Models.Dto;

namespace Mango.services.AuthAPI.Application;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}
