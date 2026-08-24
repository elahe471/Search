namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSearchResponse<T>(
    IReadOnlyCollection<T> Items,
    long TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
}
