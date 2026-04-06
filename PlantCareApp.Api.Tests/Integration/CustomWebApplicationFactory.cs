using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real SQLite DbContext registration
            var descriptor = services.SingleOrDefault(
                service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Register an in-memory SQLite database unique to this factory instance
            services.AddSqlite<AppDbContext>(
                $"Data Source=testdb_{Guid.NewGuid()}.db",
                optionsAction: options => options.UseSeeding((context, _) =>
                {
                    if (context.Set<PlantType>().Any())
                    {
                        return;
                    }

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
                }));
        });

        // Run in test environment so MigrateDb() is invoked automatically
        builder.UseEnvironment("Development");
    }
}
