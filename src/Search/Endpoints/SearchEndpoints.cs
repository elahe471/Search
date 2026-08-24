using Search.Endpoints.Contracts;

namespace Search.Endpoints
{
    public static class SearchEndpoints
    {
        public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/", Search);

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
        public static async Task<IResult> Search(
     ElasticsearchClient elasticsearch,
     [AsParameters] CatalogSearchRequest request,
     CancellationToken cancellationToken)
        {
            if (request.Page < 1)
            {
                return TypedResults.BadRequest("Page must be greater than zero.");
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

            var response = await elasticsearch.SearchAsync<CatalogItemIndex>(
                s => s
                    .Indices(CatalogItemIndex.IndexName)
                    .From(from)
                    .Size(request.PageSize)
                    .Query(q => q
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
    }
}
