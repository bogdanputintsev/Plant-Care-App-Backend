namespace PlantCareApp.Api.Dtos;

public record WeatherDto(
    string Location,
    double Temperature,
    int RainProbability,
    int Humidity,
    string Condition,
    string Description
);