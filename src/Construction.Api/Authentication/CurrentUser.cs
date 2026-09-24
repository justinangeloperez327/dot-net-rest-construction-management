using System.Security.Claims;
using Construction.Application.Abstractions.Authentication;
using Construction.Infrastructure.Authorization;

namespace Construction.Api.Authentication;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    private ClaimsPrincipal? Principal =>
        httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated == true;

    public Guid? UserId
    {
        get
        {
            string? value =
                Principal?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out Guid userId)
                ? userId
                : null;
        }
    }

    public IReadOnlySet<string> Roles =>
        Principal?
            .FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase)
        ?? new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

    public IReadOnlySet<string> Permissions =>
        Principal?
            .FindAll(
                AuthorizationClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal)
        ?? new HashSet<string>(
            StringComparer.Ordinal);
}
