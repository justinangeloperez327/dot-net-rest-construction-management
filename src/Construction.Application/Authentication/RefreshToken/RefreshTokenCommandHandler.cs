using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandler(IAuthenticationService authenticationService)
    : ICommandHandler<RefreshTokenCommand, AuthenticationTokens>
{
    public Task<Result<AuthenticationTokens>> HandleAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return authenticationService.RefreshAsync(
            command.RefreshToken,
            cancellationToken);
    }
}
