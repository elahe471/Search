using Catalog.Contracts.IntegrationEvents;

namespace Search.Infrastructure.Consumers
{
    public class CatalogItemDeletedEventConsumer(ElasticsearchClient elasticsearchClient) : IConsumer<CatalogItemDeletedEvent>
    {
        private readonly ElasticsearchClient _elasticsearchClient = elasticsearchClient;

        public async Task Consume(ConsumeContext<CatalogItemDeletedEvent> context)
        {
            var message = context.Message;

            if (message is null) return;

            var response = await _elasticsearchClient.DeleteAsync(
           CatalogItemIndex.IndexName,
           message.Slug,
           context.CancellationToken);
        }
    }
}
