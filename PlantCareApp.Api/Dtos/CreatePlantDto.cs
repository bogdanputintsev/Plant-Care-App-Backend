using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record CreatePlantDto (
    [Required][StringLength(50)] string Name,
    [Required] int TypeId,
    [Required][Range(1, 30)] int WateringIntervalDays,
    string Notes
);