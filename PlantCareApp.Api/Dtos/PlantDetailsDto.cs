namespace PlantCareApp.Api.Dtos;

public record PlantDetailsDto (
    int Id,
    string Name,
    int TypeId,
    int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes,
    DateOnly CreatedAtDate
);