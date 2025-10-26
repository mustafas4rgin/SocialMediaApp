using AutoMapper;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class UserImageProfile : Profile
{
    public UserImageProfile()
    {
        CreateMap<UserImage, UserImageDTO>().ReverseMap();
        CreateMap<CreateUserImageDTO, UserImage>().ReverseMap();
        CreateMap<UpdateUserImageDTO, UserImage>().ReverseMap();
    }
}