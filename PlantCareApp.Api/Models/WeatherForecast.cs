using System.Text.Json.Serialization;

namespace PlantCareApp.Api.Models;

public class WeatherForecast
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public double GenerationtimeMs { get; set; }

    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffsetSeconds { get; set; }

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; } = string.Empty;

    [JsonPropertyName("elevation")]
    public double Elevation { get; set; }

    [JsonPropertyName("minutely_15_units")]
    public WeatherUnits Minutely15Units { get; set; } = new();

    [JsonPropertyName("minutely_15")]
    public WeatherMinutely15 Minutely15 { get; set; } = new();
}

public class WeatherUnits
{
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("temperature_2m")]
    public string Temperature2m { get; set; } = string.Empty;

    [JsonPropertyName("precipitation_probability")]
    public string PrecipitationProbability { get; set; } = string.Empty;

    [JsonPropertyName("relative_humidity_2m")]
    public string RelativeHumidity2m { get; set; } = string.Empty;

    [JsonPropertyName("weather_code")]
    public string WeatherCode { get; set; } = string.Empty;
}

public class WeatherMinutely15
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = [];

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature2m { get; set; } = [];

    [JsonPropertyName("precipitation_probability")]
    public List<int> PrecipitationProbability { get; set; } = [];

    [JsonPropertyName("relative_humidity_2m")]
    public List<int> RelativeHumidity2m { get; set; } = [];

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = [];
}