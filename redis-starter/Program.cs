using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Redis.Starter.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));
builder.Services.AddSingleton<ICacheService, CacheService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis"), name: "redis-check", tags: new[] { "redis" })
    .AddCheck("healthy-test", () => HealthCheckResult.Healthy("Good"));

//For UI
builder.Services.AddHealthChecksUI().AddInMemoryStorage();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapWeatherForecastEndpoints();
//For health endpoint
app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

//For UI
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
});
app.MapControllers();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


public static class WeatherForecastEndpoints
{
    public static void MapWeatherForecastEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/WeatherForecast").WithTags(nameof(WeatherForecast));
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        group.MapGet("/", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();
            return TypedResults.Ok(forecast); ;
        })
        .WithName("GetAllWeatherForecasts")
        .WithOpenApi();

        group.MapGet("/{id}", (int id) =>
        {
            //return new WeatherForecast { ID = id };
        })
        .WithName("GetWeatherForecastById")
        .WithOpenApi();

        group.MapPut("/{id}", (int id, WeatherForecast input) =>
        {
            return TypedResults.NoContent();
        })
        .WithName("UpdateWeatherForecast")
        .WithOpenApi();

        group.MapPost("/", (WeatherForecast model) =>
        {
            //return TypedResults.Created($"/api/WeatherForecasts/{model.ID}", model);
        })
        .WithName("CreateWeatherForecast")
        .WithOpenApi();

        group.MapDelete("/{id}", (int id) =>
        {
            //return TypedResults.Ok(new WeatherForecast { ID = id });
        })
        .WithName("DeleteWeatherForecast")
        .WithOpenApi();
    }
}