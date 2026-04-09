using PlantCareApp.Api.Data;
using PlantCareApp.Api.Endpoints;
using PlantCareApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddHostedService<WeatherWateringService>();
builder.AddDb();

var app = builder.Build();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapPlantsEndpoints();
app.MapWeatherEndpoints();
app.MapAuthEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MigrateDb();
}

app.Run();
