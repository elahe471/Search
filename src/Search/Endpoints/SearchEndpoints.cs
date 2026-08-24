namespace Search.Endpoints
{
    public static class SearchEndpoints
    {
        public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
        {

            app.MapGet("/", SearchItems);

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
        static async Task<Results<Ok<IReadOnlyCollection<CatalogItemIndex>>, NotFound>> SearchItems(string text, ElasticsearchClient elasticsearch)
        {
            var response = await elasticsearch.SearchAsync<CatalogItemIndex>(s => s
                .Indices(CatalogItemIndex.IndexName)
                 .From(0)
                 .Size(10)
                 .Query(q => q
                 .MultiMatch(mm => mm
                 .Query(text)
                 .Fields(new[]
                 {
                     "name^4",
                     "catalogBrand^3",
                     "catalogCategory^2",
                     "description"
                 })
            .Fuzziness("AUTO"))));

            if (response.IsValidResponse)
                return TypedResults.Ok(response.Documents);

            return TypedResults.NotFound();

        }
    }
}
