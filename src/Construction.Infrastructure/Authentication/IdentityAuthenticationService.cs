using System.Security.Cryptography;
using System.Text;
using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Results;
using Construction.Domain.Audit;
using Construction.Infrastructure.Identity;
using Construction.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Authentication;

public sealed class IdentityAuthenticationService(
    UserManager<ApplicationUser> userManager,
    IPermissionService permissionService,
    ApplicationDbContext dbContext,
    JwtTokenService jwtTokenService,
    JwtOptions jwtOptions,
    TimeProvider timeProvider)
    : IAuthenticationService
{
    public async Task<Result<AuthenticationTokens>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            return InvalidCredentials();
        }

        ApplicationUser? user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return InvalidCredentials();
        }

        if (!user.IsActive)
        {
            await AddSecurityAuditAsync(
                "LoginRejected",
                user.Id,
                "Login rejected for an inactive account.",
                cancellationToken);

            return InvalidCredentials();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            await AddSecurityAuditAsync(
                "LoginBlocked",
                user.Id,
                "Login blocked because the account is locked.",
                cancellationToken);

            return Result.Failure<AuthenticationTokens>(
                ApplicationError.Unauthorized(
                    "Authentication.LockedOut",
                    "The account is temporarily locked."));
        }

        bool validPassword =
            await userManager.CheckPasswordAsync(user, password);

        if (!validPassword)
        {
            await userManager.AccessFailedAsync(user);

            await AddSecurityAuditAsync(
                "LoginFailed",
                user.Id,
                "Login failed because the supplied credentials were invalid.",
                cancellationToken);

            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);

        return await IssueTokensAsync(user, cancellationToken);
    }

    public async Task<Result<AuthenticationTokens>> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return InvalidRefreshToken();
        }

        string tokenHash = HashToken(refreshToken);
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        RefreshToken? storedToken = await dbContext.RefreshTokens
            .Include(token => token.User)
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is null || !storedToken.User.IsActive)
        {
            return InvalidRefreshToken();
        }

        if (storedToken.RevokedAtUtc is not null)
        {
            if (!string.IsNullOrWhiteSpace(
                storedToken.ReplacedByTokenHash))
            {
                await RevokeActiveTokensForUserAsync(
                    storedToken.UserId,
                    utcNow,
                    cancellationToken);

                await AddSecurityAuditAsync(
                    "RefreshTokenReuseDetected",
                    storedToken.UserId,
                    "A previously replaced refresh token was presented. Active sessions were revoked.",
                    cancellationToken);
            }
            else
            {
                await AddSecurityAuditAsync(
                    "RefreshTokenRejected",
                    storedToken.UserId,
                    "A revoked refresh token was presented.",
                    cancellationToken);
            }

            return InvalidRefreshToken();
        }

        if (!storedToken.IsActive(utcNow))
        {
            await AddSecurityAuditAsync(
                "RefreshTokenRejected",
                storedToken.UserId,
                "An expired refresh token was presented.",
                cancellationToken);

            return InvalidRefreshToken();
        }

        AuthorizationContext authorizationContext =
            await GetAuthorizationContextAsync(
                storedToken.User,
                cancellationToken);

        string newRawRefreshToken = GenerateRefreshToken();
        string newTokenHash = HashToken(newRawRefreshToken);

        storedToken.Revoke(utcNow, newTokenHash);

        var replacement = new RefreshToken(
            Guid.CreateVersion7(),
            storedToken.UserId,
            newTokenHash,
            utcNow,
            utcNow.AddDays(jwtOptions.RefreshTokenDays));

        dbContext.RefreshTokens.Add(replacement);

        (string accessToken, DateTimeOffset expiresAtUtc) =
            jwtTokenService.CreateAccessToken(
                storedToken.User,
                authorizationContext.Roles,
                authorizationContext.Permissions);

        dbContext.AuditLogs.Add(
            AuditLog.CreateSecurity(
                "TokenRefreshed",
                storedToken.UserId,
                null,
                utcNow));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthenticationTokens(
            accessToken,
            newRawRefreshToken,
            expiresAtUtc));
    }

    public async Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result.Success();
        }

        string tokenHash = HashToken(refreshToken);

        RefreshToken? storedToken = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(
                token => token.TokenHash == tokenHash,
                cancellationToken);

        if (storedToken is not null
            && storedToken.RevokedAtUtc is null)
        {
            DateTimeOffset utcNow = timeProvider.GetUtcNow();

            storedToken.Revoke(utcNow);

            dbContext.AuditLogs.Add(
                AuditLog.CreateSecurity(
                    "Logout",
                    storedToken.UserId,
                    null,
                    utcNow));

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private async Task<Result<AuthenticationTokens>> IssueTokensAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        AuthorizationContext authorizationContext =
            await GetAuthorizationContextAsync(
                user,
                cancellationToken);

        (string accessToken, DateTimeOffset expiresAtUtc) =
            jwtTokenService.CreateAccessToken(
                user,
                authorizationContext.Roles,
                authorizationContext.Permissions);

        string rawRefreshToken = GenerateRefreshToken();
        string refreshTokenHash = HashToken(rawRefreshToken);
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        dbContext.RefreshTokens.Add(new RefreshToken(
            Guid.CreateVersion7(),
            user.Id,
            refreshTokenHash,
            utcNow,
            utcNow.AddDays(jwtOptions.RefreshTokenDays)));

        dbContext.AuditLogs.Add(
            AuditLog.CreateSecurity(
                "LoginSucceeded",
                user.Id,
                null,
                utcNow));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthenticationTokens(
            accessToken,
            rawRefreshToken,
            expiresAtUtc));
    }

    private async Task<AuthorizationContext> GetAuthorizationContextAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        string[] roles =
            [.. await userManager.GetRolesAsync(user)];

        IReadOnlySet<string> permissions =
            await permissionService.GetPermissionsAsync(
                user.Id,
                cancellationToken);

        return new AuthorizationContext(
            roles,
            permissions);
    }

    private async Task RevokeActiveTokensForUserAsync(
        Guid userId,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken)
    {
        List<RefreshToken> activeTokens = await dbContext.RefreshTokens
            .Where(token =>
                token.UserId == userId
                && token.RevokedAtUtc == null
                && token.ExpiresAtUtc > revokedAtUtc)
            .ToListAsync(cancellationToken);

        foreach (RefreshToken token in activeTokens)
        {
            token.Revoke(revokedAtUtc);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AddSecurityAuditAsync(
        string action,
        Guid? userId,
        string? description,
        CancellationToken cancellationToken)
    {
        dbContext.AuditLogs.Add(
            AuditLog.CreateSecurity(
                action,
                userId,
                description,
                timeProvider.GetUtcNow()));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token) =>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));

    private static Result<AuthenticationTokens> InvalidCredentials() =>
        Result.Failure<AuthenticationTokens>(
            ApplicationError.Unauthorized(
                "Authentication.InvalidCredentials",
                "The email address or password is invalid."));

    private static Result<AuthenticationTokens> InvalidRefreshToken() =>
        Result.Failure<AuthenticationTokens>(
            ApplicationError.Unauthorized(
                "Authentication.InvalidRefreshToken",
                "The refresh token is invalid or expired."));

    private sealed record AuthorizationContext(
        IReadOnlyCollection<string> Roles,
        IReadOnlyCollection<string> Permissions);
}
