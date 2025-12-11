using Microsoft.EntityFrameworkCore;
using SocialApp.Data.Contexts;
using SocialApp.Data.Helpers;
using SocialApp.Domain.Contracts;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<List<User>> GetAllUsersAsync(string? include, CancellationToken ct = default)
    {
        var query = _context.Users
                        .Where(u => !u.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForUser(query, include);
        
        return await query.AsNoTracking().ToListAsync(ct);
    }
    public async Task<User?> GetUserByIdAsync(int id, string? include, CancellationToken ct = default)
    {
        var query = _context.Users
                        .Where(u => !u.IsDeleted && u.Id == id);

        if (!string.IsNullOrWhiteSpace(include))
            query = QueryHelper.ApplyIncludesForUser(query, include);

        return await query.FirstOrDefaultAsync(ct);
    }
}