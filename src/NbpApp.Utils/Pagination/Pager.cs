namespace NbpApp.Utils.Pagination;

public interface ISorter
{
    public Sorting[] SortingOrders { get; }
}

public interface IPager : ISorter
{
    public int CurrentPage { get; }
    public int PageSize { get; }
}

public class Pager : IPager
{
    private int _currentPage = 1;
    private int _pageSize = 10;

    public Pager()
    { }

    public Pager(int currentPage, int pageSize)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
    }


    public int CurrentPage
    {
        get => _currentPage;
        set => _currentPage = value > 0 ? value : 1;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 ? value : 10;
    }

    public Sorting[] SortingOrders { get; set; } = [];
}

public sealed record Sorting(string Field, string Direction = "desc");