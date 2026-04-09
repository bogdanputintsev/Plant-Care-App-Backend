using Microsoft.AspNetCore.Identity;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Dtos;
using PlantCareApp.Api.Models;
using PlantCareApp.Api.Services;

namespace PlantCareApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/auth");

        // POST /api/auth/register
        group.MapPost("/register", async (
            RegisterDto dto,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService) =>
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Timezone = dto.Timezone
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id);

            return Results.Ok(new AuthResponseDto(accessToken, refreshToken));
        });
        
        // POST /api/auth/login
        group.MapPost("/login", async (
            LoginDto dto,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var passwordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
            {
                return Results.Unauthorized();
            }

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id);

            return Results.Ok(new AuthResponseDto(accessToken, refreshToken));
        });
        
        // POST /api/auth/refresh
        group.MapPost("/refresh", async (
            RefreshDto dto,
            TokenService tokenService,
            UserManager<ApplicationUser> userManager,
            AppDbContext dbContext) =>
        {
            var storedToken = await tokenService.ValidateRefreshTokenAsync(dto.RefreshToken);
            if (storedToken is null)
            {
                return Results.Unauthorized();
            }

            storedToken.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var accessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(user.Id);

            return Results.Ok(new AuthResponseDto(accessToken, newRefreshToken));
        });
        
        // POST /api/auth/logout
        group.MapPost("/logout", async (
            LogoutDto dto,
            TokenService tokenService,
            AppDbContext dbContext) =>
        {
            var storedToken = await tokenService.ValidateRefreshTokenAsync(dto.RefreshToken);
            if (storedToken is null)
            {
                return Results.Unauthorized();
            }

            storedToken.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
    }

}