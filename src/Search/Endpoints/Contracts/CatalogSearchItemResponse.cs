namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSearchItemResponse(
     string Name,
     string Description,
     string CatalogCategory,
     string CatalogBrand,
     string Url,
     string? HighlightedName,
     string? HighlightedDescription);
}
