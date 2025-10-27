using AutoMapper;
using SocialApp.Application.Helpers;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Create -> Entity
        CreateMap<CreateUserDTO, User>()
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
        
        CreateMap<UpdateUserDTO, User>()
        .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
        .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
        .ForMember(dest => dest.Role, opt => opt.Ignore())
        .AfterMap((src, dest) =>
        {
            var passwordProp = typeof(UpdateUserDTO).GetProperty("Password");
            var newPassword = passwordProp?.GetValue(src) as string;
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                HashingHelper.CreatePasswordHash(newPassword, out var hash, out var salt);
                dest.PasswordHash = hash;
                dest.PasswordSalt = salt;
            }
        });

        CreateMap<User, UserDTO>().ReverseMap();
    }
}