using System.Text.Json;
using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Mapping;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Services;

public class WeatherService(IHttpClientFactory httpClientFactory)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public async Task<WeatherDto?> GetWeatherForUserAsync(ApplicationUser user)
    {
        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={user.Latitude}" +
                  $"&longitude={user.Longitude}" +
                  $"&minutely_15=temperature_2m,precipitation_probability,relative_humidity_2m,weather_code" +
                  $"&forecast_minutely_15=8" +
                  $"&timezone={Uri.EscapeDataString(user.Timezone)}";

        var client = httpClientFactory.CreateClient();
        var jsonStringResponse = await client.GetStringAsync(url);
        var forecast = JsonSerializer.Deserialize<WeatherForecast>(jsonStringResponse, JsonOptions);

        return forecast?.ToDto();
    }

    public async Task<string> GetWeatherConditionForUserAsync(ApplicationUser user)
    {
        var forecastDto = await GetWeatherForUserAsync(user);
        return forecastDto is null ? string.Empty : forecastDto.Condition;
    }
}