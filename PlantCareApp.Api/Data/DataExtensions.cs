using Microsoft.EntityFrameworkCore;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }

    public static void AddDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddSqlite<AppDbContext>(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (!context.Set<PlantType>().Any())
                {
                    context.Set<PlantType>().AddRange(
                        new PlantType { TypeName = "salad" },
                        new PlantType { TypeName = "strawberry" },
                        new PlantType { TypeName = "cucumber" },
                        new PlantType { TypeName = "mint" },
                        new PlantType { TypeName = "chilli" },
                        new PlantType { TypeName = "bell-pepper" },
                        new PlantType { TypeName = "radish" }
                    );
                    context.SaveChanges();
                }
            }));
        
    }
}