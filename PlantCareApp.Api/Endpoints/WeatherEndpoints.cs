using System.Text.Json;
using PlantCareApp.Api.Mapping;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Endpoints;

public static class WeatherEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string ForecastUrl =
        "https://api.open-meteo.com/v1/forecast?latitude=50.0755&longitude=14.4378&minutely_15=temperature_2m,precipitation_probability,relative_humidity_2m,weather_code&forecast_minutely_15=8&timezone=Europe/Prague";

    public static void MapWeatherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/weather");

        group.MapGet("/", async (IHttpClientFactory httpClientFactory) =>
        {
            var client = httpClientFactory.CreateClient();
            var jsonStringResponse = await client.GetStringAsync(ForecastUrl);
            var forecast = JsonSerializer.Deserialize<WeatherForecast>(jsonStringResponse, JsonOptions);

            return forecast?.ToDto();
        });
    }
}