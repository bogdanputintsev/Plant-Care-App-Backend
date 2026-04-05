using Microsoft.EntityFrameworkCore;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Mapping;

namespace PlantCareApp.Api.Endpoints;

public static class PlantsEndpoints
{
    private const string GetPlantByIdEndpointName = "GetPlantById";

    public static void MapPlantsEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("api/plants");
        
        // GET /api/plants
        group.MapGet("/", async (AppDbContext dbContext)
            => await dbContext.Plants
                .Include(plant => plant.Type)
                .Select(plant => plant.ToSummaryDto())
                .AsNoTracking()
                .ToListAsync()
            );
           

        // GET /api/plants/1
        group.MapGet("/{plantId:int}", async (int plantId, AppDbContext dbContext) =>
        {
            var plant = await dbContext.Plants
                .Include(plant => plant.Type)
                .AsNoTracking()
                .FirstOrDefaultAsync(plant => plant.Id == plantId);
            
            return plant is null ? Results.NotFound() : Results.Ok(plant.ToSummaryDto());
        }).WithName(GetPlantByIdEndpointName);

        // POST /api/plants
        group.MapPost("/", async (CreatePlantDto createPlantDto, AppDbContext dbContext) =>
        {
            var typeExists = await TypeIdIsValid(createPlantDto.TypeId, dbContext);
            if (!typeExists)
            {
                return Results.BadRequest("Invalid typeId: plant type does not exist.");
            }
            
            var newPlant = createPlantDto.ToEntity();
            dbContext.Plants.Add(newPlant);
            await dbContext.SaveChangesAsync();
            
            return Results.CreatedAtRoute(GetPlantByIdEndpointName, new { plantId = newPlant.Id }, newPlant.ToDetailsDto());
        });

        // PUT /api/plants/1
        group.MapPut("/{plantId:int}", async (
            int plantId, 
            UpdatePlantDto updatePlantDto, 
            AppDbContext dbContext) =>
        {
            var typeExists = await TypeIdIsValid(updatePlantDto.TypeId, dbContext);
            if (!typeExists)
            {
                return Results.BadRequest("Invalid typeId: plant type does not exist.");
            }

            if (!DateIsBeforeNow(updatePlantDto.LastWateredDate))
            {
                return Results.BadRequest("Invalid LastWateredDate: date can't be in a future.");
            }
            
            var existingPlant = await dbContext.Plants
                .FirstOrDefaultAsync(plant => plant.Id == plantId);
            
            if (existingPlant is null)
            {
                return Results.NotFound();
            }

            existingPlant.Name = updatePlantDto.Name;
            existingPlant.TypeId = updatePlantDto.TypeId;
            existingPlant.WateringIntervalDays = updatePlantDto.WateringIntervalDays;
            existingPlant.LastWateredDate = updatePlantDto.LastWateredDate;
            existingPlant.Notes = updatePlantDto.Notes;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
        
        // PATCH /api/plants/1/water
        group.MapPatch("/{plantId:int}/water", async (int plantId, AppDbContext dbContext) =>
        {
            var existingPlant = await dbContext.Plants
                .FirstOrDefaultAsync(plant => plant.Id == plantId);
            
            if (existingPlant is null)
            {
                return Results.NotFound();
            }

            existingPlant.LastWateredDate = DateOnly.FromDateTime(DateTime.UtcNow);
            await dbContext.SaveChangesAsync();

            return Results.Ok(existingPlant.ToDetailsDto());
        });

        // DELETE /api/plants/1
        group.MapDelete("/{plantId:int}", async (int plantId, AppDbContext dbContext) =>
        {
            var numberOfDeletedRows = await dbContext.Plants
                .Where(plant => plant.Id == plantId)
                .ExecuteDeleteAsync();

            return numberOfDeletedRows == 0 ? Results.NotFound() : Results.NoContent();
        }); 
    }
    
    private static async Task<bool> TypeIdIsValid(int typeId, AppDbContext dbContext)
    {
        return await dbContext.PlantTypes.AnyAsync(type => type.Id == typeId);
    }

    private static bool DateIsBeforeNow(DateOnly date)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        return date <= now;
    }
}