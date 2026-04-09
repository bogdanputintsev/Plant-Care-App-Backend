using Microsoft.AspNetCore.Identity;

namespace PlantCareApp.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Timezone { get; set; } = string.Empty;
    
}