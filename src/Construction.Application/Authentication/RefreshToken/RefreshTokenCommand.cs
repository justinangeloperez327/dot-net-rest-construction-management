using Construction.Application.Abstractions.Authentication;
using Construction.Application.Common.Messaging;

namespace Construction.Application.Authentication.RefreshToken;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<AuthenticationTokens>;
