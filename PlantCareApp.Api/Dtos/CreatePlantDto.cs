using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record CreatePlantDto (
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Type,
    [Required][Range(1, 30)] int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes
);