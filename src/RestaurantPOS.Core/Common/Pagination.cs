namespace RestaurantPOS.Core.Common;

public class PaginatedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public int Skip => (Page - 1) * PageSize;
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
