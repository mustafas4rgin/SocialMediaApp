using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface ICommentService : IGenericService<Comment>
{
    Task<IServiceResultWithData<IEnumerable<Comment>>> GetAllCommentsWithIncludesAsync(QueryParameters param, CancellationToken ct);
    Task<IServiceResultWithData<Comment>> GetCommentByIdWithIncludesAsync(int id, QueryParameters param, CancellationToken ct);
}