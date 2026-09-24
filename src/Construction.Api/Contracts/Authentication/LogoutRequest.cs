namespace Construction.Api.Contracts.Authentication;

public sealed record LogoutRequest(
    string RefreshToken);
