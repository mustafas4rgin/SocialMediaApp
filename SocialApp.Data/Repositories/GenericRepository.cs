using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
{
    private readonly AppDbContext _context;
    public GenericRepository(AppDbContext context) => _context = context;

    public IQueryable<T> GetAll(CancellationToken ct = default)
        => _context.Set<T>().AsNoTracking();

    public IQueryable<T> GetAllActive(CancellationToken ct = default)
        => _context.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);

    public async Task<T?> GetActiveByIdAsync(int id, CancellationToken ct = default)
        => await _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Set<T>().FindAsync(new object?[] { id }, ct);

    public async Task<T?> GetByIdAsNoTrackingAsync(int id, CancellationToken ct = default)
        => await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<T?> AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) return null;

        entity.CreatedAt = DateTime.UtcNow;
        await _context.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public async Task<T?> UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null || entity.Id == default) return null;

        var dbEntity = await _context.Set<T>().FindAsync(new object?[] { entity.Id }, ct);
        if (dbEntity is null) return null;

        var createdAt = dbEntity.CreatedAt;
        var isDeleted = dbEntity.IsDeleted;

        _context.Entry(dbEntity).CurrentValues.SetValues(entity);

        dbEntity.CreatedAt = createdAt;    
        dbEntity.IsDeleted = isDeleted;    
        dbEntity.UpdatedAt = DateTime.UtcNow;


        return dbEntity;
    }

    public void Delete(T entity, CancellationToken ct = default)
    {
        if (entity is null) return;
        _context.Set<T>().Remove(entity);
    }

    public void SoftDelete(T entity, CancellationToken ct = default)
    {
        if (entity is null) return;

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Attach(entity);
        _context.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        _context.Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
    }

    public void Restore(T entity, CancellationToken ct = default)
    {
        if (entity is null) return;

        entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Attach(entity);
        _context.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        _context.Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
