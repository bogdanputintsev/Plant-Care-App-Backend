using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record CreatePlantDto (
    [Required][StringLength(50)] string Name,
    [Required][StringLength(20)] string Type,
    [Required] string SunExposure,
    [Required][Range(1, 100)] int WateringIntervalDays,
    DateOnly LastWateredDate,
    string Notes
);