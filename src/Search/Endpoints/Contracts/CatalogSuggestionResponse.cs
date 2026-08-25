namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSuggestionResponse(
    IReadOnlyCollection<CatalogSuggestionItem> Items);
}
