namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSearchRequest(
    string? Query,
    int Page = 1,
    int PageSize = 10);
}
