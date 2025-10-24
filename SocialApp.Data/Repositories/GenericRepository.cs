using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : EntityBase
{
    private readonly AppDbContext _context;
    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }
    public IQueryable<T> GetAll() => _context.Set<T>().AsNoTracking();
    public IQueryable<T> GetAllActive()
        => _context.Set<T>().AsNoTracking().Where(e => !e.IsDeleted);
    public async Task<T?> GetActiveByIdAsync(int id)
    => await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

    public async Task<T?> GetByIdAsync(int id)
        => await _context.Set<T>().FindAsync(id);
    public async Task<T?> AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) return null;
        entity.Id = default;
        entity.CreatedAt = DateTime.UtcNow;
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }
    public async Task<T?> UpdateAsync(T entity)
    {
        if (entity is null || entity.Id == default) return null;

        var dbEntity = await _context.Set<T>().FindAsync(entity.Id);
        if (dbEntity is null) return null;

        _context.Entry(dbEntity).CurrentValues.SetValues(entity);
        dbEntity.UpdatedAt = DateTime.UtcNow;
        return dbEntity;
    }
    public void Delete(T entity)
    {
        if (entity is null)
            return;

        _context.Set<T>().Remove(entity);
    }
    public void SoftDelete(T entity)
    {
        if (entity is null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Attach(entity);
        _context.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        _context.Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
    }
    public void Restore(T entity)
    {
        if (entity is null) return;
        entity.IsDeleted = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _context.Attach(entity);
        _context.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        _context.Entry(entity).Property(x => x.UpdatedAt).IsModified = true;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}