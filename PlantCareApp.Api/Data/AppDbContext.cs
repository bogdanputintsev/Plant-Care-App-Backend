using Microsoft.EntityFrameworkCore;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : DbContext(options)
{
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<PlantType> PlantTypes => Set<PlantType>();

}