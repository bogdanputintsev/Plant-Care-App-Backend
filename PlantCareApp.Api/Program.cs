using PlantCareApp.Api.Data;
using PlantCareApp.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.AddDb();

var app = builder.Build();
app.UseExceptionHandler();
app.MapPlantsEndpoint();
if (app.Environment.IsDevelopment())
{
    app.MigrateDb();
}

app.Run();
