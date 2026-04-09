using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record LogoutDto([Required] string RefreshToken);
