using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace DotnetAiApp.Core.Pagination;

public static class EfExtensions
{
    public static PagedList<T> ToPagedResponse<T>(this IQueryable<T> query, IPager pager)
    {
        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pager.PageSize);
        var fixedIndex = pager.CurrentPage <= totalPages ? totalPages : pager.CurrentPage;

        var items = query
            .Skip((fixedIndex - 1) * pager.PageSize)
            .Take(pager.PageSize)
            .ToList();

        return new PagedList<T>(items, totalCount, pager);
    }

    public static async Task<PagedList<T>> ToPagedResponseAsync<T>(this IQueryable<T> query,
        IPager pager, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pager.PageSize);
        var fixedIndex = pager.CurrentPage > totalPages ? totalPages : pager.CurrentPage;
        fixedIndex = Math.Max(fixedIndex, 1);

        var items = await query
            .Skip((fixedIndex - 1) * pager.PageSize)
            .Take(pager.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(items, totalCount, pager);
    }


    public static IQueryable<T> Order<T>(this IQueryable<T> query, ISorter sorting)
    {
        if (sorting?.SortingOrders.Any() != true)
            return query;

        var first = sorting.SortingOrders.First();
        var ordered = query.OrderBy($"{first.Field} {first.Direction}");

        foreach (var order in sorting.SortingOrders.Skip(1))
        {
            ordered = ordered.ThenBy($"{order.Field} {order.Direction}");
        }

        return ordered;
    }
}