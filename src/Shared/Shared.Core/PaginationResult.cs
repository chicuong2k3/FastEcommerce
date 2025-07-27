namespace Shared.Core;

public class PaginationResult<T> where T : class
{
    public PaginationResult(int pageNumber, int pageSize, long totalRecords, IList<T> data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        Data = data;
    }

    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public long TotalRecords { get; }
    public IList<T> Data { get; } = new List<T>();
}
