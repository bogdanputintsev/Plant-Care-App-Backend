using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record LoginDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);