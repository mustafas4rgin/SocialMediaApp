public interface IGenericRepository<T> where T : EntityBase
{
    Task<List<T>> GetAllAsync(bool includeDeleted = false, CancellationToken ct = default);

    Task<T?> GetByIdAsync(
        int id,
        bool includeDeleted = false,
        bool asNoTracking = false,
        CancellationToken ct = default);

    Task<T?> AddAsync(T entity, CancellationToken ct = default);
    Task<T?> UpdateAsync(T entity, CancellationToken ct = default);

    void Delete(T entity);     
    void HardDelete(T entity);  

    void Restore(T entity);

    Task SaveChangesAsync(CancellationToken ct = default);
}
