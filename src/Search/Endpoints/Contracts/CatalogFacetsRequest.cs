namespace Search.Endpoints.Contracts
{
    public sealed record CatalogFacetsRequest(
      string? Query,
      int Size = 20);
}
