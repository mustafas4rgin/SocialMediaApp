using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class PostBrutalProfile : Profile
{
    public PostBrutalProfile()
    {
        CreateMap<PostBrutalDTO, PostBrutal>().ReverseMap();
        CreateMap<CreatePostBrutalDTO, PostBrutal>().ReverseMap();
        CreateMap<UpdatePostBrutalDTO, PostBrutal>().ReverseMap();
    }
}