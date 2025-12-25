using Microsoft.EntityFrameworkCore;
using SocialApp.Domain.Entities;

namespace SocialApp.Data.Helpers;

public static class ContextHelper
{
    public static IQueryable<T> OrderedByNewest<T>(this IQueryable<T> query) where T : EntityBase
    => query.OrderByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id);
    public static IQueryable<T> PagedQuery<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : EntityBase
    => query.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
}
