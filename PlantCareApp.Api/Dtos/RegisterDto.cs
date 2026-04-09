using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record RegisterDto(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    [Required][StringLength(100)] string FullName,
    [Required][Range(-90, 90)] double Latitude,
    [Required][Range(-180, 180)] double Longitude,
    [Required] string Timezone
);