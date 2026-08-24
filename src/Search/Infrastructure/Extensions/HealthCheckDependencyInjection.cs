using Search.Infrastructure.HealthChecks;

namespace Search.Infrastructure.Extensions
{
    public static class HealthCheckDependencyInjection
    {
        public static void HealthCheckConfigure(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks().AddCheck<ElasticsearchHealthCheck>(
            "elasticsearch",
            tags: ["ready", "elasticsearch"]);



        }
    }
}
