using AutoMapper;
using SocialApp.Domain.DTOs.Auth;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDTO, User>().ReverseMap();
    }
}