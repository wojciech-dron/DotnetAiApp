namespace NbpApp.Utils.Pagination;

public class PagedList<T> : IPager
{
    public List<T> Data { get; }
        
    public int CurrentPage { get; }
    public int PageSize { get; }
    public Sorting[] SortingOrders { get; }

    public int TotalCount { get; }
    public int TotalPages { get; }

    public PagedList(List<T> data, int totalCount, IPager pager)
    {
        Data = data;
        CurrentPage = pager.CurrentPage;
        PageSize = pager.PageSize;
        SortingOrders = pager.SortingOrders;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pager.PageSize);
    }
}