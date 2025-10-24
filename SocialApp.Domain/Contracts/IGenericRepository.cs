using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IGenericRepository<T>  where T : EntityBase
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAllActive();
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetActiveByIdAsync(int id);
    Task<T?> UpdateAsync(T Entity);
    Task<T?> AddAsync(T entity, CancellationToken ct);
    void Delete(T entity);
    void SoftDelete(T entity);
    void Restore(T entity);
    Task SaveChangesAsync();
}