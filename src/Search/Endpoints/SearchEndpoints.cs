using Elastic.Clients.Elasticsearch.QueryDsl;
using Search.Endpoints.Contracts;

namespace Search.Endpoints
{
    public static class SearchEndpoints
    {
        public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/", SearchItems);
            app.MapGet("/suggestions", GetSuggestions);

            return app;
        }
        /// <summary>
        /// Searches catalog items using Elasticsearch MultiMatch query.
        ///
        /// Search priority:
        /// 1. Name             - Boost: 4
        /// 2. CatalogBrand     - Boost: 3
        /// 3. CatalogCategory  - Boost: 2
        /// 4. Description      - Boost: 1
        ///
        /// Fuzziness is set to AUTO to tolerate common typing mistakes.
        ///
        /// Search results are ranked by Elasticsearch _score,
        /// therefore matches in higher-boosted fields appear first.
        /// </summary>
        public static async Task<IResult> SearchItems(
        ElasticsearchClient elasticsearch,
        [AsParameters] CatalogSearchRequest request,
        CancellationToken cancellationToken)
        {
            if (request.Page < 1)
            {
                return TypedResults.BadRequest(
                    "Page must be greater than zero.");
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                return TypedResults.BadRequest(
                    "PageSize must be between 1 and 100.");
            }

            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return TypedResults.BadRequest(
                    "Search query is required.");
            }

            var from = (request.Page - 1) * request.PageSize;

            var filters = new List<Action<QueryDescriptor<CatalogItemIndex>>>();

            if (!string.IsNullOrWhiteSpace(request.Brand))
            {
                filters.Add(q => q
                    .Term(t => t
                        .Field("catalogBrand.keyword")
                        .Value(request.Brand)
                        .CaseInsensitive(true)));
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                filters.Add(q => q
                    .Term(t => t
                        .Field("catalogCategory.keyword")
                        .Value(request.Category)
                        .CaseInsensitive(true)));
            }

            var response = await elasticsearch.SearchAsync<CatalogItemIndex>(
                s => s
                    .Indices(CatalogItemIndex.IndexName)
                    .From(from)
                    .Size(request.PageSize)
                    .Query(q => q
                        .Bool(b =>
                        {
                            b.Must(m => m
                                .MultiMatch(mm => mm
                                    .Query(request.Query)
                                    .Fields(new[]
                                    {
                                "name^4",
                                "catalogBrand^3",
                                "catalogCategory^2",
                                "description"
                                    })
                                    .Fuzziness(new Fuzziness("AUTO"))
                                )
                            );

                            if (filters.Count > 0)
                            {
                                b.Filter(filters.ToArray());
                            }
                        })
                    ),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                return TypedResults.Problem(
                    "An error occurred while searching Elasticsearch.");
            }

            var totalCount = response.Total;

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)request.PageSize);

            var result = new CatalogSearchResponse<CatalogItemIndex>(
                response.Documents.ToList(),
                totalCount,
                request.Page,
                request.PageSize,
                totalPages);

            return TypedResults.Ok(result);
        }
        public static async Task<IResult> GetSuggestions(
    ElasticsearchClient elasticsearch,
    [AsParameters] CatalogSuggestionRequest request,
    CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return TypedResults.BadRequest(
                    "Search query is required.");
            }

            if (request.Size < 1 || request.Size > 20)
            {
                return TypedResults.BadRequest(
                    "Size must be between 1 and 20.");
            }

            var response = await elasticsearch.SearchAsync<CatalogItemIndex>(
                s => s
                    .Indices(CatalogItemIndex.IndexName)
                    .Size(request.Size)
                    .Query(q => q
                        .MatchBoolPrefix(m => m
                            .Field(x => x.Name)
                            .Query(request.Query)
                        )
                    ),
                cancellationToken);

            if (!response.IsValidResponse)
            {
                var error =
          response.ElasticsearchServerError?.Error?.Reason
          ?? response.DebugInformation;

                return TypedResults.Problem(
                    title: "Elasticsearch suggestion search failed",
                    detail: error);
            }

            var suggestions = response.Documents
                                      .Select(x => new CatalogSuggestionItem(
                                          x.Name,
                                          x.CatalogCategory,
                                          x.CatalogBrand,
                                          x.Url)).ToArray();

            var result = new CatalogSuggestionResponse(suggestions);

            return TypedResults.Ok(result);
        }
    }
}
