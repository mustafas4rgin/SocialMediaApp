using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;
using SocialApp.Domain.Parameters;

namespace SocialApp.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
{
    private readonly AppDbContext _context;
    private DbSet<T> Set => _context.Set<T>();

    public GenericRepository(AppDbContext context) => _context = context;

    protected IQueryable<T> Query(bool includeDeleted, bool asNoTracking)
    {
        IQueryable<T> query = Set;

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        if (asNoTracking)
            query = query.AsNoTracking();

        return query;
    }

    public async Task<List<T>> GetAllAsync(QueryParameters param, bool includeDeleted = false, CancellationToken ct = default)
        => await Query(includeDeleted, asNoTracking: true)
            .PagedQuery(param.PageNumber, param.PageSize)
            .ToListAsync(ct);

    public async Task<T?> GetByIdAsync(
        int id,
        bool includeDeleted = false,
        bool asNoTracking = false,
        CancellationToken ct = default)
        => await Query(includeDeleted, asNoTracking)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<T?> AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) return null;

        await Set.AddAsync(entity, ct);
        return entity;
    }

    public async Task<T?> UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null || entity.Id == default) return null;

        var dbEntity = await Set.FirstOrDefaultAsync(x => x.Id == entity.Id, ct);
        if (dbEntity is null) return null;

        var createdAt = dbEntity.CreatedAt;
        var isDeleted = dbEntity.IsDeleted;
        var deletedAt = dbEntity.DeletedAt;

        _context.Entry(dbEntity).CurrentValues.SetValues(entity);

        dbEntity.CreatedAt = createdAt;
        dbEntity.IsDeleted = isDeleted;
        dbEntity.DeletedAt = deletedAt;

        dbEntity.UpdatedAt = DateTime.UtcNow;

        return dbEntity;
    }

    public void Delete(T entity)
    {
        if (entity is null) return;
        Set.Remove(entity); 
    }

    public void HardDelete(T entity)
    {
        if (entity is null) return;

        Set.IgnoreQueryFilters()
           .Where(x => x.Id == entity.Id)
           .ExecuteDelete();
    }

    public void Restore(T entity)
    {
        if (entity is null) return;

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedAt = DateTime.UtcNow;

        Set.Attach(entity);

        _context.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        _context.Entry(entity).Property(x => x.DeletedAt).IsModified = true;
        _context.Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
