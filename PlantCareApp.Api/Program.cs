using PlantCareApp.Api.Data;
using PlantCareApp.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.AddDb();

var app = builder.Build();
app.MapPlantsEndpoint();
app.MigrateDb();

app.Run();
