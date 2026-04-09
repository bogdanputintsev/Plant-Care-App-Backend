using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlantCareApp.Api.Models;
using PlantCareApp.Api.Services;

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
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddSqlite<AppDbContext>(
            connectionString,
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (context.Set<PlantType>().Any()) return;
                
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
        
        builder.Services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<AppDbContext>();
        
        var secret = builder.Configuration["Jwt:Secret"]!;
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddScoped<TokenService>();
    }
}