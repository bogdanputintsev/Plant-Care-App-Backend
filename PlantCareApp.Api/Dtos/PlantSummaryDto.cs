namespace PlantCareApp.Api.Dtos;

public record PlantSummaryDto (
    int Id,
    string Name,
    int TypeId,
    string Type,
    int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes,
    DateOnly CreatedAtDate
);