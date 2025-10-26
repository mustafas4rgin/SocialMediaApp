using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class PostImageProfile : Profile
{
    public PostImageProfile()
    {
        CreateMap<PostImage, PostImageDTO>().ReverseMap();
        CreateMap<CreatePostImageDTO, PostImage>().ReverseMap();
        CreateMap<UpdatePostImageDTO, PostImage>().ReverseMap();
    }
}