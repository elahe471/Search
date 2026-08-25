namespace Search.Endpoints.Contracts
{
    public sealed record CatalogSuggestionRequest(
    string? Query,
    int Size = 5);
}
