namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSuggestionItem(
     string Name,
     string Category,
     string Brand,
     string Url);
}
