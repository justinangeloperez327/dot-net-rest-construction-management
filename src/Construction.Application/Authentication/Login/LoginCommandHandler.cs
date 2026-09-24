using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Authentication.Login;

public sealed class LoginCommandHandler(IAuthenticationService authenticationService)
    : ICommandHandler<LoginCommand, AuthenticationTokens>
{
    public Task<Result<AuthenticationTokens>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        return authenticationService.LoginAsync(
            command.Email,
            command.Password,
            cancellationToken);
    }
}
