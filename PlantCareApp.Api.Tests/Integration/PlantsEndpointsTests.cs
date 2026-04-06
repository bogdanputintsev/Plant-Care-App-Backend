using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Dtos;

namespace PlantCareApp.Api.Tests.Integration;

public class PlantsEndpointsTests(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public async Task InitializeAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await dbContext.Plants.ExecuteDeleteAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetAllPlants_ReturnsEmptyList_WhenNoPlantsExist()
    {
        var response = await _client.GetAsync("api/plants");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var plants = await response.Content.ReadFromJsonAsync<List<PlantSummaryDto>>();
        plants.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePlant_ReturnsCreatedPlant_WhenRequestIsValid()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var createDto = new CreatePlantDto(
            "New Plant",
            1,
            3,
            "Let's hope this plant is created..."
        );
        
        var response = await _client.PostAsJsonAsync("api/plants", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await response.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        createdPlant.Name.Should().Be("New Plant");
        createdPlant.TypeId.Should().Be(1);
        createdPlant.WateringIntervalDays.Should().Be(3);
        createdPlant.CreatedAtDate.Should().Be(currentDate);
        createdPlant.LastWateredDate.Should().Be(currentDate);
        createdPlant.Notes.Should().Be("Let's hope this plant is created...");
    }

    [Fact]
    public async Task CreatePlant_ReturnsBadRequest_WhenTypeIdDoesNotExist()
    {
        const int invalidTypeId = 9999;
        
        var createDto = new CreatePlantDto(
            "Mysterious plant with not existing type",
            invalidTypeId,
            3,
            "Should not be created"
        );

        var response = await _client.PostAsJsonAsync("api/plants", createDto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetPlantById_ReturnsPlant_WhenPlantExists()
    {
        // 1. Create a plant via POST, read the returned PlantDetailsDto to get its id
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var createDto = new CreatePlantDto(
            "New Plant",
            1,
            3,
            "Let's hope this plant could be found..."
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. GET api/plants/{id}
        var getResponse = await _client.GetAsync($"api/plants/{createdPlantId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var receivedPlant = await getResponse.Content.ReadFromJsonAsync<PlantSummaryDto>();
        receivedPlant.Should().NotBeNull();
        
        // 3. Check that all properties are matching.
        receivedPlant.Name.Should().Be("New Plant");
        receivedPlant.TypeId.Should().Be(1);
        receivedPlant.WateringIntervalDays.Should().Be(3);
        receivedPlant.CreatedAtDate.Should().Be(currentDate);
        receivedPlant.LastWateredDate.Should().Be(currentDate);
        receivedPlant.Notes.Should().Be("Let's hope this plant could be found...");
    }

    [Fact]
    public async Task GetPlantById_ReturnsNotFound_WhenPlantDoesNotExist()
    {
        const int someUnknownId = 9999;
        var getResponse = await _client.GetAsync($"api/plants/{someUnknownId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePlant_ReturnsNotFound_WhenPlantDoesNotExists()
    {
        const int someUnknownId = 9999;
        var deleteResponse = await _client.DeleteAsync($"api/plants/{someUnknownId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DeletePlant_ReturnsNoContent_WhenPlantExists()
    {
        // 1. Create a new plant.
        var createDto = new CreatePlantDto(
            "New Plant",
            1,
            3,
            ""
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. Delete a new plant
        var deleteResponse = await _client.DeleteAsync($"api/plants/{createdPlantId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // 3. Check that plant does not exist anymore.
        var getResponse = await _client.GetAsync($"api/plants/{createdPlantId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdatePlant_ReturnsNotFound_WhenPlantDoesNotExist()
    {
        const int someUnknownId = 9999;
        
        var updateDto = new UpdatePlantDto(
            "Plant name",
            1,
            3,
            new DateOnly(2025, 5, 5),
            "Notes");

        var putResponse = await _client.PutAsJsonAsync($"api/plants/{someUnknownId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdatePlant_ReturnsNoContent_WhenRequestIsValid()
    {
        // 1. Create a new plant.
        var createDto = new CreatePlantDto(
            "Plant with boring name",
            1,
            3,
            "Old notes"
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. Update the plant name.
        var updateDto = new UpdatePlantDto(
            "New name for a plant",
            2,
            4,
            new DateOnly(2025, 5, 5),
            "Updated notes");

        var putResponse = await _client.PutAsJsonAsync($"api/plants/{createdPlantId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // 3. Check that plant contains old watered date.
        var getResponseBeforeWatering = await _client.GetAsync($"api/plants/{createdPlantId}");
        getResponseBeforeWatering.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedPlant = await getResponseBeforeWatering.Content.ReadFromJsonAsync<PlantSummaryDto>();
        updatedPlant.Should().NotBeNull();
        updatedPlant.Name.Should().Be(updateDto.Name);
        updatedPlant.TypeId.Should().Be(updateDto.TypeId);
        updatedPlant.WateringIntervalDays.Should().Be(updateDto.WateringIntervalDays);
        updatedPlant.LastWateredDate.Should().Be(updateDto.LastWateredDate);
        updatedPlant.Notes.Should().Be(updateDto.Notes);
    }
    
    [Fact]
    public async Task UpdatePlant_ReturnsBadRequest_WhenTypeIdDoesNotExist()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        const int unknownTypeId = 9999;
        
        // 1. Create a new plant.
        var createDto = new CreatePlantDto(
            "Some plant",
            1,
            3,
            "Old notes"
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. Update the plant name.
        var updateDto = new UpdatePlantDto(
            createDto.Name,
            unknownTypeId,  // Here we use invalid typeId
            createDto.WateringIntervalDays,
            currentDate,
            createDto.Notes);
        
        var putResponse = await _client.PutAsJsonAsync($"api/plants/{createdPlantId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdatePlant_ReturnsBadRequest_WhenLastWateredDateIsInFuture()
    {
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        
        // 1. Create a new plant.
        var createDto = new CreatePlantDto(
            "Some plant",
            1,
            3,
            "Old notes"
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. Update the plant name.
        var updateDto = new UpdatePlantDto(
            createDto.Name,
            createDto.TypeId,
            createDto.WateringIntervalDays,
            futureDate, // Here we use date in a future 
            createDto.Notes);
        
        var putResponse = await _client.PutAsJsonAsync($"api/plants/{createdPlantId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WaterPlant_ReturnsUpdatedPlant_WhenPlantExists()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var someOldDate = new DateOnly(2000, 1, 1);
        
        // 1. Create a new plant.
        var createDto = new CreatePlantDto(
            "New Plant",
            1,
            3,
            ""
        );
        
        var postResponse = await _client.PostAsJsonAsync("api/plants", createDto);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdPlant = await postResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        createdPlant.Should().NotBeNull();
        
        var createdPlantId = createdPlant.Id;
        
        // 2. Update the last watered date to be old.
        var updateDto = new UpdatePlantDto(
            createdPlant.Name,
            createdPlant.TypeId,
            createdPlant.WateringIntervalDays,
            someOldDate,
            createdPlant.Notes);

        var putResponse = await _client.PutAsJsonAsync($"api/plants/{createdPlantId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // 3. Check that plant contains old watered date.
        var getResponseBeforeWatering = await _client.GetAsync($"api/plants/{createdPlantId}");
        getResponseBeforeWatering.StatusCode.Should().Be(HttpStatusCode.OK);

        var plantBeforeWatering = await getResponseBeforeWatering.Content.ReadFromJsonAsync<PlantSummaryDto>();
        plantBeforeWatering.Should().NotBeNull();
        plantBeforeWatering.LastWateredDate.Should().Be(someOldDate);
            
        // 4. Patch request to update LastWateredDate
        var patchResponse = await _client.PatchAsync($"api/plants/{createdPlantId}/water", null);
        patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var wateredPlant = await patchResponse.Content.ReadFromJsonAsync<PlantDetailsDto>();
        wateredPlant.Should().NotBeNull();

        wateredPlant.LastWateredDate.Should().Be(currentDate);
        
        // 5. Also check that get response also returns updated LastWateredDate.
        var getResponseAfterWatering = await _client.GetAsync($"api/plants/{createdPlantId}");
        getResponseAfterWatering.StatusCode.Should().Be(HttpStatusCode.OK);

        var plantAfterWatering = await getResponseAfterWatering.Content.ReadFromJsonAsync<PlantSummaryDto>();
        plantAfterWatering.Should().NotBeNull();
        plantAfterWatering.LastWateredDate.Should().Be(currentDate);
    }
}
