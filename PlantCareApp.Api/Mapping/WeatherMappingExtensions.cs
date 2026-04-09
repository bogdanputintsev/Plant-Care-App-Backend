using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Mapping;

public static class WeatherMappingExtensions
{
    extension(WeatherForecast forecast)
    {
        public WeatherDto ToDto()
        {
            var data = forecast.Minutely15;
            var location = forecast.Timezone.Split('/').LastOrDefault() ?? forecast.Timezone;

            return new WeatherDto(
                Location: location,
                Temperature: data.Temperature2m.FirstOrDefault(),
                RainProbability: data.PrecipitationProbability.FirstOrDefault(),
                Humidity: data.RelativeHumidity2m.FirstOrDefault(),
                Condition: WeatherCondition.MapCondition(data.WeatherCode.FirstOrDefault()),
                Description: MapDescription(data.WeatherCode.FirstOrDefault())
            );
        }
    }

    public static class WeatherCondition
    {
        private const string Sunny = "sunny";
        private const string PartlyCloudy = "partly-cloudy";
        private const string Cloudy = "cloudy";
        private const string Rainy = "rainy";
        private const string Stormy = "stormy";

        public static string MapCondition(int wmoCode) => wmoCode switch
        {
            0 => Sunny,
            1 or 2 => PartlyCloudy,
            3 or 45 or 48 => Cloudy,
            >= 51 and <= 67 => Rainy,
            >= 71 and <= 77 => Cloudy,
            >= 80 and <= 82 => Rainy,
            85 or 86 => Cloudy,
            95 or 96 or 99 => Stormy,
            _ => Cloudy
        };
        
        public static bool IsRaining(string? weatherCondition)
        {
            return weatherCondition is Rainy or Stormy;
        }
    };
    
    private static string MapDescription(int wmoCode) => wmoCode switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 => "Fog",
        48 => "Icy fog",
        51 => "Light drizzle",
        53 => "Moderate drizzle",
        55 => "Dense drizzle",
        61 => "Slight rain",
        63 => "Moderate rain",
        65 => "Heavy rain",
        66 => "Light freezing rain",
        67 => "Heavy freezing rain",
        71 => "Slight snowfall",
        73 => "Moderate snowfall",
        75 => "Heavy snowfall",
        77 => "Snow grains",
        80 => "Slight rain showers",
        81 => "Moderate rain showers",
        82 => "Violent rain showers",
        85 => "Slight snow showers",
        86 => "Heavy snow showers",
        95 => "Thunderstorm",
        96 => "Thunderstorm with slight hail",
        99 => "Thunderstorm with heavy hail",
        _ => "Unknown"
    };
}