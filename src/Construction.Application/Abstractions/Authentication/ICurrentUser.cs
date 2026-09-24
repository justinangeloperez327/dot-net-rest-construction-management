namespace Construction.Application.Abstractions.Authentication;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    IReadOnlySet<string> Roles { get; }

    IReadOnlySet<string> Permissions { get; }
}
