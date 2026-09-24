using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Messaging;

namespace Construction.Application.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<AuthenticationTokens>;
