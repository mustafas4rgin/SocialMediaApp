using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Application.Interfaces;

public interface IGenericService<T> 
where T : EntityBase
{
    Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync(QueryParameters param, CancellationToken ct = default);
    Task<IServiceResultWithData<IEnumerable<T>>> GetAllActiveAsync(QueryParameters param, CancellationToken ct = default);
    Task<IServiceResultWithData<T>> AddAsync(T entity, CancellationToken ct = default);
    Task<IServiceResultWithData<T>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IServiceResultWithData<T>> GetActiveByIdAsync(int id, CancellationToken ct = default);
    Task<IServiceResult> UpdateAsync(T entity, CancellationToken ct = default);
    Task<IServiceResult> DeleteByIdAsync(int id, CancellationToken ct = default);
    Task<IServiceResult> RestoreAsync(int id, CancellationToken ct = default);
}