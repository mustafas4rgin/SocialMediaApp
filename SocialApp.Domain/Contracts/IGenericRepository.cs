using SocialApp.Domain.Entities;

namespace SocialApp.Domain.Contracts;

public interface IGenericRepository
{
    IQueryable<T> GetAll<T>() where T : EntityBase;
    Task<T?> GetByIdAsync<T>(int id) where T : EntityBase;
    Task<T?> UpdateAsync<T>(T Entity) where T : EntityBase;
    Task<T?> AddAsync<T>(T entity) where T : EntityBase;
    void Delete<T>(T entity) where T : EntityBase;
    void SoftDelete<T>(T entity) where T : EntityBase;
    void Restore<T>(T entity) where T : EntityBase;
    Task SaveChangesAsync();
}