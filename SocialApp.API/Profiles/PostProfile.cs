using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostDTO, Post>().ReverseMap();
        CreateMap<CreatePostDTO, Post>().ReverseMap();
        CreateMap<UpdatePostDTO, Post>().ReverseMap();
    }
}