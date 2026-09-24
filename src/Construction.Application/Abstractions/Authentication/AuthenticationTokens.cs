namespace Construction.Application.Abstractions.Authentication;

public sealed record AuthenticationTokens(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
