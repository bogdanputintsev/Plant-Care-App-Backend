using System.ComponentModel.DataAnnotations;

namespace PlantCareApp.Api.Dtos;

public record RefreshDto([Required] string RefreshToken);
