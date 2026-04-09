using System.Security.Claims;
using System.Text.Json;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Mapping;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Endpoints;

public static class WeatherEndpoints
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static void MapWeatherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/weather").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal principal, IHttpClientFactory httpClientFactory, AppDbContext dbContext) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = (ApplicationUser?)await dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var url = $"https://api.open-meteo.com/v1/forecast" +
                      $"?latitude={user.Latitude}" +
                      $"&longitude={user.Longitude}" +
                      $"&minutely_15=temperature_2m,precipitation_probability,relative_humidity_2m,weather_code" +
                      $"&forecast_minutely_15=8" +
                      $"&timezone={Uri.EscapeDataString(user.Timezone)}";

            var client = httpClientFactory.CreateClient();
            var jsonStringResponse = await client.GetStringAsync(url);
            var forecast = JsonSerializer.Deserialize<WeatherForecast>(jsonStringResponse, JsonOptions);

            return Results.Ok(forecast?.ToDto());
        });

    }
}