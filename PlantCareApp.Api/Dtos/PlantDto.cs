namespace PlantCareApp.Api.Dtos;

public record PlantDto (
    int Id,
    string Name,
    string Type,
    string SunExposure,
    int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes,
    DateOnly CreatedAtDate
);