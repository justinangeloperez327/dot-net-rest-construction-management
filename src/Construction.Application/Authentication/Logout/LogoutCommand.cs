using Construction.Application.Common.Messaging;

namespace Construction.Application.Authentication.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand;
