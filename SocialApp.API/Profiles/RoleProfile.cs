using AutoMapper;
using SocialApp.API.DTOs.List;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<CreateRoleDTO, Role>().ReverseMap();
        CreateMap<RoleDTO, Role>().ReverseMap();
        CreateMap<Role,RoleDTO>().ReverseMap();
        CreateMap<UpdateRoleDTO, Role>().ReverseMap();
    }
}