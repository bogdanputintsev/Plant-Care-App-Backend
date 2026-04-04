namespace PlantCareApp.Api.Models;

public class Plant
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public PlantType? Type { get; set; }

    public int TypeId { get; set; }

    public int WateringIntervalDays { get; set; }

    public DateOnly LastWateredDate { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DateOnly CreatedAtDate { get; set; }
    
}