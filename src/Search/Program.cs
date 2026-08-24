using Search;
using Search.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.BindAppSettings();
builder.BrokerConfigure();
builder.ElasticSearchConfigure();



builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGroup("/api/v1/search")
   .WithTags("Search APIs")
   .MapSearchEndpoints();

app.Run();




