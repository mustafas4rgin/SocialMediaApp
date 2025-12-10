using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IGenericRepository<T>  where T : EntityBase
{
    Task<List<T>>GetAllAsync(CancellationToken ct = default);
    Task<List<T>> GetAllActiveAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<T?> GetActiveByIdAsync(int id, CancellationToken ct = default);
    Task<T?> UpdateAsync(T Entity, CancellationToken ct = default);
    Task<T?> AddAsync(T entity, CancellationToken ct = default);
    void Delete(T entity, CancellationToken ct = default);
    void SoftDelete(T entity, CancellationToken ct = default);
    void Restore(T entity, CancellationToken ct = default);
    Task<T?> GetByIdAsNoTrackingAsync(int id, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}