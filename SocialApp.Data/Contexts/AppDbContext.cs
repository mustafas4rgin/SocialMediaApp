using Microsoft.EntityFrameworkCore;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentResponse> Responses { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostBrutal> Brutals { get; set; }
    public DbSet<PostImage> PostImages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserImage> UserImages { get; set; }
}