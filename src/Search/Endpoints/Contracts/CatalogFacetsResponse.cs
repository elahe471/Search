namespace Search.Endpoints.Contracts
{
    public sealed record CatalogFacetsResponse(
        IReadOnlyCollection<CatalogFacetItem> Brands,
        IReadOnlyCollection<CatalogFacetItem> Categories);
}
