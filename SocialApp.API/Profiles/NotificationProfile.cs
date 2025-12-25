using AutoMapper;
using SocialApp.Domain.DTOs.Create;
using SocialApp.Domain.DTOs.List;
using SocialApp.Domain.DTOs.Update;
using SocialApp.Domain.Entities;

namespace SocialApp.API.Profiles;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<NotificationDTO, Notification>().ReverseMap();
        CreateMap<CreateNotificationDTO, Notification>().ReverseMap();
        CreateMap<UpdateNotificationDTO, Notification>().ReverseMap();
    }
}