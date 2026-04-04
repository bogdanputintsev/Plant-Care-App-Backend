using PlantCareApp.Api.Dtos;

namespace PlantCareApp.Api.Endpoints;

public static class PlantsEndpoints
{
    private const string GetPlantsEndpointName = "GetPlants";
    
    private static readonly List<PlantDto> Plants =
    [
        new (1, "Butterhead Lettuce", "Salad", "Partial Shade", 2, new DateOnly(2026, 4, 3), "Keep soil consistently moist. Harvest outer leaves first.", new DateOnly(2026, 4, 3)),
        new (2, "Garden Strawberry", "Strawberry", "Full Sun", 3, new DateOnly(2026, 4, 3), "Remove runners to focus energy on fruit production.", new DateOnly(2026, 4, 3)),
        new (3, "Spearmint", "Mint", "Partial Shade", 2, new DateOnly(2025, 4, 3), "Keep in a container to prevent invasive spreading.", new DateOnly(2026, 4, 3)),
    ];

    public static void MapPlantsEndpoint(this WebApplication app)
    {
        var group = app.MapGroup("api/plants");
        
        // GET /api/plants
        group.MapGet("/", () => Plants)
            .WithName(GetPlantsEndpointName);

        // GET /api/plants/1
        group.MapGet("/{plantId:int}", (int plantId) =>
        {
            var plant = Plants.Find(plant => plant.Id == plantId);   
            return plant is null ? Results.NotFound() : Results.Ok(plant);
        });

        // POST /plants
        group.MapPost("/", (CreatePlantDto createPlantDto) =>
        {
            PlantDto newPlant = new(
                Plants.Count + 1,
                createPlantDto.Name,
                createPlantDto.Type,
                createPlantDto.SunExposure,
                createPlantDto.WateringIntervalDays,
                createPlantDto.LastWateredDate,
                createPlantDto.Notes,
                DateOnly.FromDateTime(DateTime.Now)
            );

            Plants.Add(newPlant);

            return Results.CreatedAtRoute(GetPlantsEndpointName, new { id = newPlant.Id }, newPlant);
        });

        // PUT /api/plants/1
        group.MapPut("/{plantId:int}", (int plantId, UpdatePlantDto updatePlantDto) =>
        {
            var plantIndex = Plants.FindIndex(plant => plant.Id == plantId);

            if (plantIndex == -1)
            {
                return Results.NotFound();
            }
    
            Plants[plantIndex] = new PlantDto(
                plantId,
                updatePlantDto.Name,
                updatePlantDto.Type,
                updatePlantDto.SunExposure,
                updatePlantDto.WateringIntervalDays,
                updatePlantDto.LastWateredDate,
                updatePlantDto.Notes,
                updatePlantDto.CreatedAtDate);

            return Results.NoContent();
        });
        
        // PATCH /api/plants/1/water
        group.MapPatch("/{plantId:int}/water", (int plantId) =>
        {
            var plantIndex = Plants.FindIndex(plant => plant.Id == plantId);

            if (plantIndex == -1)
            {
                return Results.NotFound();
            }

            var updatedPlant = Plants[plantIndex] with { LastWateredDate = DateOnly.FromDateTime(DateTime.Now) };
            Plants[plantIndex] = updatedPlant;

            return Results.Ok(updatedPlant);
        });

        // DELETE /api/plants/1
        group.MapDelete("/{plantId:int}", (int plantId) =>
        {
            Plants.RemoveAll(plant => plant.Id == plantId);

            return Results.NoContent();
        });

    }
}