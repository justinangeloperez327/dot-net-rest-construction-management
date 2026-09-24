using Construction.Api.Configuration;
using Construction.Api.Contracts.Authentication;
using Construction.Api.Extensions;
using Construction.Application.Authentication.Login;
using Construction.Application.Authentication.Logout;
using Construction.Application.Authentication.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Route(ApiRoutes.Version1 + "/auth")]
public sealed class AuthenticationController(
    LoginCommandHandler loginHandler,
    RefreshTokenCommandHandler refreshHandler,
    LogoutCommandHandler logoutHandler)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await loginHandler.HandleAsync(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await refreshHandler.HandleAsync(
            new RefreshTokenCommand(request.RefreshToken),
            cancellationToken);

        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(
        LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await logoutHandler.HandleAsync(
            new LogoutCommand(request.RefreshToken),
            cancellationToken);

        return result.ToActionResult();
    }
}
