using System.Data.Common;
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
    public IQueryable<T> GetAll()
    {
        return _context.Set<T>();
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);

        if (entity is null)
            return null;

        return entity;
    }
    public async Task<T?> AddAsync(T entity)
    {
        entity.Id = default;

        if (entity is null)
            return null;

        await _context.Set<T>().AddAsync(entity);
        entity.CreatedAt = DateTime.UtcNow;

        return entity;
    }
    public async Task<T?> UpdateAsync(T entity)
    {
        if (entity.Id == default)
            return null;

        var dbEntity = await _context.Set<T>().FindAsync(entity.Id);

        if (dbEntity is null)
            return null;

        entity.UpdatedAt = DateTime.UtcNow;

        _context.Update(entity);

        return entity;
    }
    public void Delete(T entity)
    {
        if (entity is null)
            return;

        _context.Set<T>().Remove(entity);
    }
    public void SoftDelete(T entity)
    {
        if (entity is null)
            return;

        entity.IsDeleted = true;
        _context.Update(entity);
    }
    public void Restore(T entity)
    {
        if (entity is null)
            return;

        entity.IsDeleted = false;
        _context.Update(entity);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}