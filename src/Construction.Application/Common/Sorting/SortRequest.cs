namespace Construction.Application.Common.Sorting;

public sealed record SortRequest(
    string SortBy,
    SortDirection Direction = SortDirection.Ascending);
