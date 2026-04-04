namespace PlantCareApp.Api.Dtos;

public record PlantDto (
    int Id,
    string Name,
    string Type,
    int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes,
    DateOnly CreatedAtDate
);