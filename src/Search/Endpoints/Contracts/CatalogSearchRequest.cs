namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSearchRequest(
       string? Query,
       string? Brand,
       string? Category,
       int Page = 1,
       int PageSize = 10);
}
