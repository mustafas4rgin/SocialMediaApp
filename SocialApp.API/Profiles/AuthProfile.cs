using AutoMapper;
using SocialApp.Application.Helpers;
using SocialApp.Domain.DTOs.Auth;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, RegisterDTO>().ReverseMap()
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
        .ForMember(dest => dest.Role, opt => opt.Ignore())
        .AfterMap((src, dest) =>
        {
            if (!string.IsNullOrWhiteSpace(src.Password))
            {
                HashingHelper.CreatePasswordHash(src.Password, out var hash, out var salt);
                dest.PasswordHash = hash;
                dest.PasswordSalt = salt;
            }
        });
    }
}