using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Search.Infrastructure.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
        public static async Task WriteResponse(
            HttpContext context,
            HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration.TotalMilliseconds,
                    error = entry.Value.Exception?.Message
                })
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
