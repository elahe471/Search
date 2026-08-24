using Catalog.Contracts.IntegrationEvents;

namespace Search.Infrastructure.Consumers;

public class CatalogItemAddedEventConsumer(ElasticsearchClient elasticsearchClient) : IConsumer<CatalogItemAddedEvent>
{
    private readonly ElasticsearchClient _elasticsearchClient = elasticsearchClient;

    public async Task Consume(ConsumeContext<CatalogItemAddedEvent> context)
    {
        var message = context.Message;

        if (message is null) return;

        var itemIndex = new CatalogItemIndex
        {
            CatalogBrand = message.CatalogBrand,
            CatalogCategory = message.CatalogCategory,
            Description = message.Description,
            Url = message.DetailUrl,
            Name = message.Name,
            Id = message.Slug,
        };

        //var result = await _elasticsearchClient.Indices.ExistsAsync(CatalogItemIndex.IndexName);

        //if (!result.Exists)
        //{
        //    await _elasticsearchClient.Indices
        //        .CreateAsync<CatalogItemIndex>(index: CatalogItemIndex.IndexName);
        //}

        //await _elasticsearchClient.IndexAsync(itemIndex, index: CatalogItemIndex.IndexName);

        var existsResponse =
       await _elasticsearchClient.Indices.ExistsAsync(
           CatalogItemIndex.IndexName);

        if (!existsResponse.Exists)
        {
            var createResponse =
                await _elasticsearchClient.Indices.CreateAsync(
                    CatalogItemIndex.IndexName);

            if (!createResponse.IsValidResponse)
            {
                throw new Exception(
                    $"Create index failed: {createResponse.DebugInformation}");
            }
        }

        var indexResponse = await _elasticsearchClient.IndexAsync(
            itemIndex,
            x => x
                .Index(CatalogItemIndex.IndexName)
                .Id(itemIndex.Id));

        if (!indexResponse.IsValidResponse)
        {
            throw new Exception(
                $"Index document failed: {indexResponse.DebugInformation}");
        }
    }
}
