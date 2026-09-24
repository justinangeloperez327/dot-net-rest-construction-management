using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Construction.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Construction.Infrastructure.Authentication;

public sealed class JwtTokenService(
    JwtOptions options,
    TimeProvider timeProvider)
{
    public (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(
        ApplicationUser user,
        IReadOnlyCollection<string> roles)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(roles);

        DateTimeOffset now = timeProvider.GetUtcNow();
        DateTimeOffset expiresAt = now.AddMinutes(options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(options.SigningKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}
