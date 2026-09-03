using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OrderEngine.Api.Auth;

public static class JwtTokenService
{
    public static string FixedEmail => Environment.GetEnvironmentVariable("ORDERENGINE_AUTH_EMAIL")
        ?? throw new InvalidOperationException("ORDERENGINE_AUTH_EMAIL is not configured.");

    public static string FixedPassword => Environment.GetEnvironmentVariable("ORDERENGINE_AUTH_PASSWORD")
        ?? throw new InvalidOperationException("ORDERENGINE_AUTH_PASSWORD is not configured.");

    public static bool ValidateCredentials(string email, string password)
        => string.Equals(email, FixedEmail, StringComparison.OrdinalIgnoreCase)
           && password == FixedPassword;

    public static string CreateToken(string email, string issuer, string audience, string key)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, email)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
