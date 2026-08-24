namespace Search.Endpoints
{
    public static class SearchEndpoints
    {
        public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
        {
           
            app.MapGet("/", SearchItems);

            return app;
        }

        static async Task<Results<Ok<IReadOnlyCollection<CatalogItemIndex>>, NotFound>> SearchItems(string q, ElasticsearchClient elasticsearch)
        {
            var response = await elasticsearch.SearchAsync<CatalogItemIndex>(s => s
                .Indices(CatalogItemIndex.IndexName)
                .From(0)
                .Size(10)
                .Query(qr =>
                     qr.Fuzzy(t => t.Field(x => x.Description).Value(q)))
            );

            if (response.IsValidResponse)
                return TypedResults.Ok(response.Documents);

            return TypedResults.NotFound();

        }
    }
}
