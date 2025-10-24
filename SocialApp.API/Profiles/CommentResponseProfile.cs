using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class CommentResponseProfile : Profile
{
    public CommentResponseProfile()
    {
        CreateMap<CreateCommentResponseDTO, CommentResponse>().ReverseMap();
        CreateMap<UpdateCommentResponseDTO, CommentResponse>().ReverseMap();
        CreateMap<CommentResponseDTO, CommentResponse>().ReverseMap();
    }
}