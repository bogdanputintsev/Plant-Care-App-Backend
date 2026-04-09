using Microsoft.EntityFrameworkCore;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Mapping;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Services;

public class WeatherWateringService(IServiceProvider serviceProvider, ILogger<WeatherWateringService> logger) 
    : BackgroundService
{
    private const int TimerFrequencyMinutes = 15;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(TimerFrequencyMinutes));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                logger.LogDebug("Checking weather conditions...");
                await using var scope = serviceProvider.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherService>();
                
                await CheckWeatherForAllUsers(dbContext, weatherService, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected on app shutdown — safe to ignore
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Weather watering check failed at {Time}", DateTimeOffset.UtcNow);
        }
    }

    private async Task CheckWeatherForAllUsers(AppDbContext dbContext, WeatherService weatherService, CancellationToken cancellationToken)
    {
        var allUsers = await dbContext.Users.ToListAsync(cancellationToken: cancellationToken);
        
        foreach (var user in allUsers.OfType<ApplicationUser>())
        {
            try
            {
                var weatherForUser = await weatherService.GetWeatherForUserAsync(user);

                if (WeatherMappingExtensions.WeatherCondition.IsRaining(weatherForUser?.Condition))
                {
                    await WaterAllUserPlants(user, dbContext, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Failed to process watering for user {UserId}", user.Id);
            }
        }
    }

    private static async Task WaterAllUserPlants(ApplicationUser user, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
                
        await dbContext.Plants
            .Where(plant => plant.UserId == user.Id)
            .ExecuteUpdateAsync(setter => setter.SetProperty(plant => plant.LastWateredDate, currentDate), cancellationToken);
    }
}