using FluentAssertions;
using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Mapping;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Tests.Unit.Mapping;

public class PlantMappingTests
{
    [Fact]
    public void ToSummaryDto_MapsAllFields_Correctly()
    {
        var plantType = new PlantType()
        {
            Id = 1,
            TypeName = "Salad"
        };
        
        var plant = new Plant()
        {
            Id = 1,
            Name = "Name",
            Type = plantType,
            TypeId = plantType.Id,
            WateringIntervalDays = 3,
            CreatedAtDate = new DateOnly(2025, 5, 5),
            LastWateredDate = new DateOnly(2026, 1, 1),
            Notes = "Simple plant"
        };

        var plantSummaryDto = plant.ToSummaryDto();
        plantSummaryDto.Id.Should().Be(plant.Id);
        plantSummaryDto.Name.Should().Be(plant.Name);
        plantSummaryDto.TypeId.Should().Be(plant.TypeId);
        plantSummaryDto.Type.Should().Be(plant.Type.TypeName);
        plantSummaryDto.WateringIntervalDays.Should().Be(plant.WateringIntervalDays);
        plantSummaryDto.LastWateredDate.Should().Be(plant.LastWateredDate);
        plantSummaryDto.Notes.Should().Be(plant.Notes);
        plantSummaryDto.CreatedAtDate.Should().Be(plant.CreatedAtDate);
    }

    [Fact]
    public void ToDetailsDto_MapsAllFields_Correctly()
    {
        var plantType = new PlantType()
        {
            Id = 1,
            TypeName = "Salad"
        };
        
        var plant = new Plant()
        {
            Id = 1,
            Name = "Name",
            Type = plantType,
            TypeId = plantType.Id,
            WateringIntervalDays = 3,
            CreatedAtDate = new DateOnly(2025, 5, 5),
            LastWateredDate = new DateOnly(2026, 1, 1),
            Notes = "Simple plant"
        };

        var plantSummaryDto = plant.ToDetailsDto();
        plantSummaryDto.Id.Should().Be(plant.Id);
        plantSummaryDto.Name.Should().Be(plant.Name);
        plantSummaryDto.TypeId.Should().Be(plant.TypeId);
        plantSummaryDto.WateringIntervalDays.Should().Be(plant.WateringIntervalDays);
        plantSummaryDto.LastWateredDate.Should().Be(plant.LastWateredDate);
        plantSummaryDto.Notes.Should().Be(plant.Notes);
        plantSummaryDto.CreatedAtDate.Should().Be(plant.CreatedAtDate);
    }

    [Fact]
    public void ToEntity_MapsAllFields_AndSetsDatesTodayUtc()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        const string userId = "test-user-id";
        var createDto = new CreatePlantDto("Name", 1, 3, "Simple plant");

        var plant = createDto.ToEntity(userId);
        plant.Name.Should().Be(createDto.Name);
        plant.TypeId.Should().Be(createDto.TypeId);
        plant.WateringIntervalDays.Should().Be(createDto.WateringIntervalDays);
        plant.LastWateredDate.Should().Be(currentDate);
        plant.Notes.Should().Be(createDto.Notes);
        plant.CreatedAtDate.Should().Be(currentDate);
    }
}
