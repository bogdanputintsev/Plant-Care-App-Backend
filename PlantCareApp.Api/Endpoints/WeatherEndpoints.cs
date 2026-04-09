using System.Security.Claims;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Models;
using PlantCareApp.Api.Services;

namespace PlantCareApp.Api.Endpoints;

public static class WeatherEndpoints
{
    public static void MapWeatherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/weather").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal principal, AppDbContext dbContext, WeatherService weatherService) =>
        {
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var user = (ApplicationUser?)await dbContext.Users.FindAsync(userId);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var weather = await weatherService.GetWeatherForUserAsync(user);
            return weather is null 
                ? Results.Problem("Failed to receive weather data from the external resource") 
                : Results.Ok(weather);
        });

    }
}