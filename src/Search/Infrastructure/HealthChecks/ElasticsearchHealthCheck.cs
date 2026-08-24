
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Search.Infrastructure.HealthChecks;

public sealed class ElasticsearchHealthCheck(
    ElasticsearchClient elasticsearch)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await elasticsearch.PingAsync(cancellationToken);

            return response.IsValidResponse
                ? HealthCheckResult.Healthy("Elasticsearch is available.")
                : HealthCheckResult.Unhealthy("Elasticsearch is unavailable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Elasticsearch health check failed.",
                exception);
        }
    }
}