using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Interfaces;

public interface IGenericService<T> 
where T : EntityBase
{
    Task<IServiceResultWithData<IEnumerable<T>>> GetAllAsync();
    Task<IServiceResult> AddAsync(T entity);
    Task<IServiceResultWithData<T>> GetByIdAsync(int id);
    Task<IServiceResult> UpdateAsync(T entity);
    Task<IServiceResult> DeleteAsync(T entity);
}