using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class FollowProfile : Profile
{
    public FollowProfile()
    {
        CreateMap<CreateFollowDTO, Follow>().ReverseMap();
        CreateMap<UpdateFollowDTO, Follow>().ReverseMap();
        CreateMap<FollowDTO, Follow>().ReverseMap();
    }
}