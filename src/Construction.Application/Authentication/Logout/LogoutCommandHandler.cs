using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Authentication.Logout;

public sealed class LogoutCommandHandler(IAuthenticationService authenticationService)
    : ICommandHandler<LogoutCommand>
{
    public Task<Result> HandleAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return authenticationService.RevokeAsync(
            command.RefreshToken,
            cancellationToken);
    }
}
