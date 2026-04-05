using PlantCareApp.Api.Data;
using PlantCareApp.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();
builder.AddDb();

var app = builder.Build();
app.UseExceptionHandler();
app.MapPlantsEndpoints();
app.MapWeatherEndpoints();
if (app.Environment.IsDevelopment())
{
    app.MigrateDb();
}

app.Run();
