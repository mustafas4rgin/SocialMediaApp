using System.Reflection.Emit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApp.Domain.Entities;

namespace SocialApp.Application.Helpers;

public static class QueryHelper
{
    public static IQueryable<CommentResponse> ApplyIncludesForCommentResponse(IQueryable<CommentResponse> query, string include)
    {
        if (StringHelper.EmptyCheck(include))
            return query;

        var includes = StringHelper.Includes(include);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "comment": query = query.Include(cr => cr.Comment); break;
                case "post": query = query.Include(cr => cr.Post); break;
                case "all":
                    query = query
                                    .Include(cr => cr.Post)
                                    .Include(cr => cr.Comment);
                    break;
                default: break;
            }
        }

        return query;
    }
    public static IQueryable<Role> ApplyIncludesForRole(IQueryable<Role> query, string include)
    {
        if (StringHelper.EmptyCheck(include))
            return query;

        var includes = StringHelper.Includes(include);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "users": query = query.Include(r => r.Users); break;
                case "all": query = query.Include(r => r.Users); break;
                default : break;
            }
        }

        return query;
    }
    public static IQueryable<Like> ApplyIncludesForLike(IQueryable<Like> query, string include)
    {
        if (StringHelper.EmptyCheck(include))
            return query;

        var includes = StringHelper.Includes(include);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "post": query = query.Include(l => l.Post); break;
                case "user": query = query.Include(l => l.User); break;
                case "all":
                    query = query
                    .Include(l => l.Post)
                    .Include(l => l.User);
                    break;
            }
        }

        return query;
    }
    public static IQueryable<Follow> ApplyIncludesForFollow(IQueryable<Follow> query, string include)
    {
        if (StringHelper.EmptyCheck(include))
            return query;

        var includes = StringHelper.Includes(include);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "follower": query = query.Include(f => f.Follower); break;
                case "following": query = query.Include(f => f.Following); break;
                case "all":
                    query = query
                    .Include(f => f.Follower)
                    .Include(f => f.Following);
                    break;
            }
        }

        return query;
    }
    public static IQueryable<Comment> ApplyIncludesForComment(IQueryable<Comment> query, string include)
    {
        if (StringHelper.EmptyCheck(include))
            return query;
        
        var includes = StringHelper.Includes(include);

        foreach (var inc in includes.Select(i => i.Trim().ToLower()))
        {
            switch (inc)
            {
                case "post": query = query.Include(c => c.Post); break;
                case "user": query = query.Include(c => c.User); break;
                case "all":
                    query = query
                    .Include(c => c.Post)
                    .Include(c => c.User);
                    break;
            }
        }

        return query;
    }
}