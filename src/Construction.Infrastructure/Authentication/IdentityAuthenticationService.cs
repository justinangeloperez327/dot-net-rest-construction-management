using System.Security.Cryptography;
using System.Text;
using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Results;
using Construction.Infrastructure.Identity;
using Construction.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Authentication;

public sealed class IdentityAuthenticationService(
    UserManager<ApplicationUser> userManager,
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

        if (user is null || !user.IsActive)
        {
            return InvalidCredentials();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return Result.Failure<AuthenticationTokens>(
                ApplicationError.Unauthorized(
                    "Authentication.LockedOut",
                    "The account is temporarily locked."));
        }

        bool validPassword = await userManager.CheckPasswordAsync(user, password);

        if (!validPassword)
        {
            await userManager.AccessFailedAsync(user);
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
            if (!string.IsNullOrWhiteSpace(storedToken.ReplacedByTokenHash))
            {
                await RevokeActiveTokensForUserAsync(
                    storedToken.UserId,
                    utcNow,
                    cancellationToken);
            }

            return InvalidRefreshToken();
        }

        if (!storedToken.IsActive(utcNow))
        {
            return InvalidRefreshToken();
        }

        IReadOnlyCollection<string> roles =
            (await userManager.GetRolesAsync(storedToken.User)).ToArray();

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
            jwtTokenService.CreateAccessToken(storedToken.User, roles);

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
            storedToken.Revoke(timeProvider.GetUtcNow());
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    private async Task<Result<AuthenticationTokens>> IssueTokensAsync(
        ApplicationUser user,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<string> roles = (await userManager.GetRolesAsync(user)).ToArray();

        (string accessToken, DateTimeOffset expiresAtUtc) =
            jwtTokenService.CreateAccessToken(user, roles);

        string rawRefreshToken = GenerateRefreshToken();
        string refreshTokenHash = HashToken(rawRefreshToken);
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        dbContext.RefreshTokens.Add(new RefreshToken(
            Guid.CreateVersion7(),
            user.Id,
            refreshTokenHash,
            utcNow,
            utcNow.AddDays(jwtOptions.RefreshTokenDays)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthenticationTokens(
            accessToken,
            rawRefreshToken,
            expiresAtUtc));
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

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string HashToken(string token) =>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));

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
}
