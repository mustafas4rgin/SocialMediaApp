using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class LikeProfile : Profile
{
    public LikeProfile()
    {
        CreateMap<CreateLikeDTO, Like>().ReverseMap();
        CreateMap<UpdateLikeDTO, Like>().ReverseMap();
        CreateMap<Like, LikeDTO>().ReverseMap();
    }
}