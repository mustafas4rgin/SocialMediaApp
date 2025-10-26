using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDTO, User>().ReverseMap();
        CreateMap<UpdateUserDTO, User>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
    }
}