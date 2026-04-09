using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Mapping;

public static class PlantMappingExtensions
{
    extension(Plant plant)
    {
        public PlantSummaryDto ToSummaryDto()
        {
            return new PlantSummaryDto(
                plant.Id,
                plant.Name,
                plant.TypeId,
                plant.Type!.TypeName,
                plant.WateringIntervalDays,
                plant.LastWateredDate,
                plant.Notes,
                plant.CreatedAtDate
            );
        }

        public PlantDetailsDto ToDetailsDto()
        {
            return new PlantDetailsDto(
                plant.Id,
                plant.Name,
                plant.TypeId,
                plant.WateringIntervalDays,
                plant.LastWateredDate,
                plant.Notes,
                plant.CreatedAtDate
            );
        }
    }

    extension(CreatePlantDto createPlantDto)
    {
        public Plant ToEntity(string userId)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            
            return new Plant
            {
                Name = createPlantDto.Name,
                TypeId = createPlantDto.TypeId,
                WateringIntervalDays = createPlantDto.WateringIntervalDays,
                LastWateredDate = currentDate,
                Notes = createPlantDto.Notes,
                CreatedAtDate = currentDate,
                UserId = userId
            };

        }
    }
}