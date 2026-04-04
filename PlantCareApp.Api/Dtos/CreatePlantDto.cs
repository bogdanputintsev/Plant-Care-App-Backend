using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record CreatePlantDto (
    [Required][StringLength(50)] string Name,
    [Required][Range(0, 60)] int TypeId,
    [Required][Range(1, 30)] int WateringIntervalDays,
    string Notes,
    DateOnly CreatedAtDate
);