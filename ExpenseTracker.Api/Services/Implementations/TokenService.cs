using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ExpenseTracker.Api.Domain.Entities;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ExpenseTracker.Api.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(ApplicationUser user)
    {
        var secretKey = Environment.GetEnvironmentVariable("JWT_KEY")
            ?? _configuration["Jwt:Key"]
            ?? "ExpenseTrackerSuperSecretSecureKeyForJwtToken2026!";
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
            ?? _configuration["Jwt:Issuer"]
            ?? "ExpenseTrackerApi";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
            ?? _configuration["Jwt:Audience"]
            ?? "ExpenseTrackerClient";
        var durationMinutesStr = Environment.GetEnvironmentVariable("JWT_DURATION_MINUTES")
            ?? _configuration["Jwt:DurationInMinutes"];
        var durationMinutes = int.TryParse(durationMinutesStr, out var minutes) ? minutes : 1440; // 24 hours

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrEmpty(user.FullName))
        {
            claims.Add(new Claim("FullName", user.FullName));
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(durationMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expiresAtUtc);
    }
}

