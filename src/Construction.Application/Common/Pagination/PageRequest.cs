namespace Construction.Application.Common.Pagination;

public sealed record PageRequest(
    int PageNumber = PageRequest.DefaultPageNumber,
    int PageSize = PageRequest.DefaultPageSize)
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;
    public const int MaximumPageSize = 100;
}
