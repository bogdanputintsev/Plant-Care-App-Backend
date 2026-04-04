using Microsoft.EntityFrameworkCore;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Endpoints;

public static class PlantsEndpoints
{
    private const string GetPlantsEndpointName = "GetPlants";

    public static void MapPlantsEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("api/plants");
        
        // TODO: Why here we use PlantDto and not PlantDetailsDto?
        // TODO: Learn more about  .AsNoTracking()
        
        // GET /api/plants
        group.MapGet("/", async (AppDbContext dbContext)
            => await dbContext.Plants
                .Include(plant => plant.Type)
                .Select(plant => new PlantDto(
                    plant.Id,
                    plant.Name,
                    plant.Type!.TypeName,
                    plant.WateringIntervalDays,
                    plant.LastWateredDate,
                    plant.Notes,
                    plant.CreatedAtDate
                ))
                .AsNoTracking()
                .ToListAsync()
            );
           

        // GET /api/plants/1
        group.MapGet("/{plantId:int}", async (int plantId, AppDbContext dbContext) =>
        {
            var plant = await dbContext.Plants.FindAsync(plantId);
            
            return plant is null ? Results.NotFound() : Results.Ok(
                new PlantDetailsDto(
                    plant.Id,
                    plant.Name,
                    plant.TypeId,
                    plant.WateringIntervalDays,
                    plant.LastWateredDate,
                    plant.Notes,
                    plant.CreatedAtDate
                )
            );
        }).WithName(GetPlantsEndpointName);

        // POST /plants
        group.MapPost("/", async (CreatePlantDto createPlantDto, AppDbContext dbContext) =>
        {
            Plant plant = new()
            {
                Name = createPlantDto.Name,
                TypeId = createPlantDto.TypeId,
                WateringIntervalDays = createPlantDto.WateringIntervalDays,
                LastWateredDate = createPlantDto.CreatedAtDate,
                Notes = createPlantDto.Notes,
                CreatedAtDate = createPlantDto.CreatedAtDate
            };

            dbContext.Plants.Add(plant);
            await dbContext.SaveChangesAsync();
            
            // TODO: Does this dto make sense?
            PlantDetailsDto plantDetailsDto = new(
                plant.Id,
                plant.Name,
                plant.TypeId,
                plant.WateringIntervalDays,
                plant.LastWateredDate,
                plant.Notes,
                plant.CreatedAtDate
            );

            return Results.CreatedAtRoute(GetPlantsEndpointName, new { id = plant.Id }, plantDetailsDto);
        });

        // PUT /api/plants/1
        group.MapPut("/{plantId:int}", async (
            int plantId, 
            UpdatePlantDto updatePlantDto, 
            AppDbContext dbContext) =>
        {
            var existingPlant = await dbContext.Plants.FindAsync(plantId);
            
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
            var existingPlant = await dbContext.Plants.FindAsync(plantId);

            if (existingPlant is null)
            {
                return Results.NotFound();
            }

            existingPlant.LastWateredDate = DateOnly.FromDateTime(DateTime.Now);
            await dbContext.SaveChangesAsync();

            return Results.Ok(new PlantDetailsDto(
                existingPlant.Id,
                existingPlant.Name,
                existingPlant.TypeId,
                existingPlant.WateringIntervalDays,
                existingPlant.LastWateredDate,
                existingPlant.Notes,
                existingPlant.CreatedAtDate
            ));
        });

        // DELETE /api/plants/1
        group.MapDelete("/{plantId:int}", async (int plantId, AppDbContext dbContext) =>
        {
            await dbContext.Plants
                .Select(plant => plant.Id == plantId)
                .ExecuteDeleteAsync();

            return Results.NoContent();
        });

    }
}