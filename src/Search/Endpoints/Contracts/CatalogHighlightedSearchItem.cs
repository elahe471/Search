namespace Search.Endpoints.Contracts
{
    public sealed record CatalogHighlightedSearchItem(
     string Name,
     string Description,
     string CatalogCategory,
     string CatalogBrand,
     string Url,
     string? HighlightedName,
     string? HighlightedDescription);
}
