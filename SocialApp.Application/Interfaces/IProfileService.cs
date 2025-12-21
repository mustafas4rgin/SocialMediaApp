using SocialApp.Domain.Contracts;
using SocialApp.Domain.DTOs;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IProfileService
{
    Task<IServiceResultWithData<ProfileDTO>> GetProfileAsync(int userId, QueryParameters param, CancellationToken ct = default);
}