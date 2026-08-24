using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Search;
using Search.Endpoints;
using Search.Infrastructure.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.BindAppSettings();
builder.BrokerConfigure();
builder.ElasticSearchConfigure();
builder.HealthCheckConfigure();


builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGroup("/api/v1/search")
   .WithTags("Search APIs")
   .MapSearchEndpoints();

app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = _ => false, //dont check elasticsearch for liveness, only readiness
          ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = HealthCheckResponseWriter.WriteResponse
});

app.Run();




