using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlantCareApp.Api.Data;
using PlantCareApp.Api.Models;

namespace PlantCareApp.Api.Services;

public class TokenService(AppDbContext dbContext, IConfiguration configuration)
{
    public string GenerateAccessToken(ApplicationUser user)
      {
          var secret = configuration["Jwt:Secret"]!;
          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
          var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          var expiryMinutes = configuration.GetValue<int>("Jwt:AccessTokenExpiryMinutes");

          var claims = new[]
          {
              new Claim(JwtRegisteredClaimNames.Sub, user.Id),
              new Claim(JwtRegisteredClaimNames.Email, user.Email!),
              new Claim(JwtRegisteredClaimNames.Name, user.FullName)
          };

          var token = new JwtSecurityToken(
              claims: claims,
              expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
              signingCredentials: credentials
          );

          return new JwtSecurityTokenHandler().WriteToken(token);
      }

      public async Task<string> GenerateRefreshTokenAsync(string userId)
      {
          var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
          var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
          var expiryDays = configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays");

          var refreshToken = new RefreshToken
          {
              TokenHash = tokenHash,
              UserId = userId,
              ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
              IsRevoked = false
          };

          dbContext.RefreshTokens.Add(refreshToken);
          await dbContext.SaveChangesAsync();

          return rawToken;
      }

      public async Task<RefreshToken?> ValidateRefreshTokenAsync(string rawToken)
      {
          var tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

          return await dbContext.RefreshTokens
              .FirstOrDefaultAsync(storedToken =>
                  storedToken.TokenHash == tokenHash
                  && !storedToken.IsRevoked
                  && storedToken.ExpiresAt > DateTime.UtcNow);
      }
}