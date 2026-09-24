namespace Construction.Api.Contracts.Authentication;

public sealed record CurrentUserResponse(
    Guid UserId,
    IReadOnlySet<string> Roles,
    IReadOnlySet<string> Permissions);
