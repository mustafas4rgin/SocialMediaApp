using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Helpers;

public static class QueryHelper
{
    public static IQueryable<Role> ApplyIncludesForRole(IQueryable<Role> query, string include)
    {
        if (string.IsNullOrEmpty(include))
            return query;

        var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "users": query.Include(r => r.Users); break;
                case "all":
                    query
                                .Include(r => r.Users); break;
            }
        }

        return query;
    }
    public static IQueryable<Like> ApplyIncludesForLike(IQueryable<Like> query, string include)
    {
        if (string.IsNullOrEmpty(include))
            return query;

        var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "post": query.Include(l => l.Post); break;
                case "user": query.Include(l => l.User); break;
                case "all":
                    query
                    .Include(l => l.Post)
                    .Include(l => l.User);
                    break;
            }
        }

        return query;
    }
    public static IQueryable<Follow> ApplyIncludesForFollow(IQueryable<Follow> query, string include)
    {
        if (string.IsNullOrEmpty(include))
            return query;

        var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "follower": query.Include(f => f.Follower); break;
                case "following": query.Include(f => f.Following); break;
                case "all":
                    query
                    .Include(f => f.Follower)
                    .Include(f => f.Following);
                    break;
            }
        }

        return query;
    }
    public static IQueryable<Comment> ApplyIncludesForComment(IQueryable<Comment> query, string include)
    {
        if (string.IsNullOrEmpty(include))
            return query;
        
        var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "post": query.Include(c => c.Post); break;
                case "user": query.Include(c => c.User); break;
                case "all":
                    query
                    .Include(c => c.Post)
                    .Include(c => c.User);
                    break;
            }
        }

        return query;
    }
}