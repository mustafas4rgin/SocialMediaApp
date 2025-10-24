using AutoMapper;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<CreateCommentDTO, Comment>().ReverseMap();
        CreateMap<UpdateCommentDTO, Comment>().ReverseMap();
        CreateMap<CommentDTO, Comment>().ReverseMap();
    }
}