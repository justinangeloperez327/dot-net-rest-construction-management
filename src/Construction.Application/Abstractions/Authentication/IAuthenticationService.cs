using Construction.Application.Common.Results;

namespace Construction.Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<Result<AuthenticationTokens>> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationTokens>> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}
